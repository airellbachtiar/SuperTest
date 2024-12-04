using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Grpc.Net.Client;
using System.Diagnostics;
using TestBus;
using TrafficTest.Models;

namespace TrafficTest
{
    /// <summary>
    /// Manages test automation hooks for traffic light system testing.
    /// Handles application lifecycle, UI automation, and traffic light state monitoring.
    /// </summary>
    [Binding]
    public partial class TrafficTestHooks
    {
        private static Application _app;
        private static UIA3Automation _automation;
        private static Application _appSim;
        private static GrpcChannel _channel;
        private static CancellationTokenSource? _cancellationTokenSource;

        /// <summary>
        /// gRPC client for communication with the test simulation
        /// </summary>
        public static TestSim.TestSimClient Client { get; private set; }
        /// <summary>
        /// Historical record of traffic light states during test execution
        /// </summary>
        public static List<TrafficLightState> TrafficLightStates { get; private set; } = new();

        /// <summary>
        /// Initializes the test environment before feature execution.
        /// Starts both Traffic and TrafficSim applications and establishes gRPC connection.
        /// </summary>
        [BeforeFeature]
        public static void BeforeTestRun()
        {
            InitializeTrafficApplication();
            InitializeTrafficSimulationApplication();
            InitializeGrpcConnection();
        }

        /// <summary>
        /// Prepares the test environment before each scenario.
        /// Starts traffic light state observation.
        /// </summary>
        [BeforeScenario]
        public static void BeforeScenario()
        {
            InitializeStateTracking();
            StartTrafficLightObservation();
        }

        /// <summary>
        /// Cleans up resources after each scenario.
        /// Stops observation and resets state.
        /// </summary>
        [AfterScenario]
        public static void AfterScenario()
        {
            CleanupScenarioResources();
        }

        /// <summary>
        /// Performs final cleanup after feature execution.
        /// Closes applications and releases resources.
        /// </summary>
        [AfterFeature]
        public static void AfterTestRun()
        {
            CleanupTestEnvironment();
        }

        /// <summary>
        /// Simulates a click on a button in the main window of the Traffic application.
        /// </summary>
        /// <param name="automationId">The AutomationId of the target button</param>
        /// <exception cref="Exception">Thrown when the button or main window cannot be found</exception>
        public static void ClickButton(string automationId)
        {
            var mainWindow = _app.GetMainWindow(_automation)
                            ?? throw new Exception("Main window could not be found.");

            // Find the button using its AutomationId
            var button = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsButton()
                         ?? throw new Exception($"Button with AutomationId '{automationId}' could not be found.");

            // Simulate button click
            button.Invoke();
            Task.Delay(1000).Wait();
        }

        /// <summary>
        /// Monitors and records traffic light states continuously.
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
        /// Waits until a specific traffic light is in the desired state or until timeout is reached.
        /// This method blocks execution until the light state matches the expected value.
        /// </summary>
        /// <param name="lightName">The name of the traffic light (e.g., "CarYellow").</param>
        /// <param name="lightState">The desired state of the light (e.g., "On", "Off").</param>
        /// <param name="timeoutInSecond">The maximum time to wait for the light to reach the desired state. Defaults to 120 seconds.</param>
        /// <exception cref="TimeoutException">Thrown when the desired state is not reached within the timeout period.</exception>
        public static void WaitUntilTheLightIsInThatState(string lightName, string lightState, int timeoutInSecond = 120)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (true)
            {
                if (stopwatch.Elapsed.TotalSeconds > timeoutInSecond)
                {
                    stopwatch.Stop();
                    throw new TimeoutException($"The light '{lightName}' did not reach the state '{lightState}' within 2 minutes.");
                }

                if (lightState != "CarGreen" || lightState == "PedestrianRed")
                {
                    Client.PressRequestPedestrianWalkButton(new Empty());
                }

                var lastState = TrafficLightStates.LastOrDefault();
                if (lastState == null)
                {
                    continue;
                }

                if (IsLightInState(lastState, lightName, lightState))
                {
                    stopwatch.Stop();
                    return;
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Checks if a specific traffic light is in the desired state.
        /// </summary>
        /// <param name="state">The current traffic light state.</param>
        /// <param name="lightName">The name of the traffic light.</param>
        /// <param name="lightState">The desired state of the light.</param>
        /// <returns>True if the light is in the desired state; otherwise, false.</returns>
        /// <exception cref="Exception">Thrown for unsupported light names</exception>
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
