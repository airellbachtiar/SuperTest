using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Grpc.Net.Client;
using System.Diagnostics;
using TestBus;
using TrafficTest.Models;

#pragma warning disable CS8618
#pragma warning disable CS8602

namespace TrafficTest
{
    [Binding]
    public partial class TrafficTestHooks
    {
        public static Application App { get; private set; }
        public static UIA3Automation Automation { get; private set; }
        public static Application AppSim { get; set; }
        public static TestSim.TestSimClient Client { get; private set; }
        private static GrpcChannel _channel;
        public static List<TrafficLightState> TrafficLightStates { get; private set; } = new();
        private static CancellationTokenSource? _cancellationTokenSource;

        [BeforeFeature]
        public static void BeforeTestRun()
        {
            // Start and attach to the Traffic application
            var trafficAppPath = GetApplicationPath("Traffic", "net8.0-windows", "Traffic.exe");
            App = StartAndAttachToApplication(trafficAppPath);
            FocusMainWindow(App);

            // Start and attach to the TrafficSim application
            var trafficSimAppPath = GetApplicationPath("TrafficSim", "net6.0-windows", "TrafficSim.exe");
            AppSim = StartAndAttachToApplication(trafficSimAppPath);

            // Wait for the applications to load
            Task.Delay(2000).Wait();

            // Set up gRPC channel and client
            _channel = GrpcChannel.ForAddress($"http://localhost:{Test.Default.TestPort}");
            Client = new TestSim.TestSimClient(_channel);
        }

        [BeforeScenario]
        public static void BeforeScenario()
        {
            TrafficLightStates = new List<TrafficLightState>();
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => ObserveTrafficLight(_cancellationTokenSource.Token));
        }

        [AfterScenario]
        public static void AfterScenario()
        {
            ClickButton("StopButton");

            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            Task.Delay(1000).Wait();
        }

        [AfterFeature]
        public static void AfterTestRun()
        {
            try
            {
                CloseApplication(App, "Traffic");
                CloseApplication(AppSim, "TrafficSim");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while closing the application: {ex.Message}");
            }
            finally
            {
                CleanupResources();
            }
        }

        /// <summary>
        /// Simulates a click on a button in the main window of the Traffic application.
        /// The button is identified by its AutomationId.
        /// </summary>
        /// <param name="automationId">The unique AutomationId of the button.</param>
        public static void ClickButton(string automationId)
        {
            var mainWindow = App.GetMainWindow(Automation)
                            ?? throw new Exception("Main window could not be found.");

            // Find the button using its AutomationId
            var button = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsButton()
                         ?? throw new Exception($"Button with AutomationId '{automationId}' could not be found.");

            // Simulate button click
            button.Invoke();
            Task.Delay(1000).Wait();
        }

        /// <summary>
        /// Continuously observes the traffic light states and records their changes.
        /// This method runs on a separate thread until canceled.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the observation.</param>
        private static void ObserveTrafficLight(CancellationToken cancellationToken)
        {
            Stopwatch s = new();
            s.Start();
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    TrafficLightStates.Add(new()
                    {
                        CarRed = Client.GetCarRedLightState(new Empty(), cancellationToken: cancellationToken),
                        CarYellow = Client.GetCarYellowLightState(new Empty(), cancellationToken: cancellationToken),
                        CarGreen = Client.GetCarGreenLightState(new Empty(), cancellationToken: cancellationToken),
                        PedestrianRed = Client.GetPedestrianRedLightState(new Empty(), cancellationToken: cancellationToken),
                        PedestrianGreen = Client.GetPedestrianGreenLightState(new Empty(), cancellationToken: cancellationToken),
                        TimeStamp = s.Elapsed.TotalSeconds
                    });
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Observation stopped.");
            }
            finally
            {
                s.Stop();
            }
        }

        /// <summary>
        /// Waits until a specific traffic light is in the desired state.
        /// This method blocks execution until the light state matches the expected value.
        /// </summary>
        /// <param name="lightName">The name of the traffic light (e.g., "CarYellow").</param>
        /// <param name="lightState">The desired state of the light (e.g., "On", "Off").</param>
        public static void WaitUntilTheLightIsInThatState(string lightName, string lightState)
        {
            while (true)
            {
                var lastState = TrafficLightStates.LastOrDefault();
                if (lastState == null)
                {
                    continue;
                }

                if (IsLightInState(lastState, lightName, lightState)) return;
            }
        }

        /// <summary>
        /// Checks if a specific traffic light is in the desired state.
        /// </summary>
        /// <param name="state">The current traffic light state.</param>
        /// <param name="lightName">The name of the traffic light.</param>
        /// <param name="lightState">The desired state of the light.</param>
        /// <returns>True if the light is in the desired state; otherwise, false.</returns>
        private static bool IsLightInState(TrafficLightState state, string lightName, string lightState) =>
            lightName switch
            {
                "CarYellow" => state.CarYellow.LightState == lightState,
                "PedestrianGreen" => state.PedestrianGreen.LightState == lightState,
                "CarRed" => state.CarRed.LightState == lightState,
                "PedestrianRed" => state.PedestrianRed.LightState == lightState,
                "CarGreen" => state.CarGreen.LightState == lightState,
                _ => throw new Exception($"Light '{lightName}' is not supported."),
            };
    }
}
