using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Grpc.Net.Client;
using System.Diagnostics;
using TestBus;

#pragma warning disable CS8618
#pragma warning disable CS8602

namespace TrafficTest
{
    [Binding]
    public class SpecFlowHooks
    {
        public static Application App { get; private set; }
        public static UIA3Automation Automation { get; private set; }

        public static Application AppSim { get; set; }

        public static TestSim.TestSimClient Client { get; private set; }
        private static GrpcChannel _channel;

        [BeforeScenario]
        public static void BeforeTestRun()
        {
            // Start and attach to the Traffic application
            var trafficAppPath = GetApplicationPath("Traffic", "net8.0-windows", "Traffic.exe");
            App = StartAndAttachToApplication(trafficAppPath);
            FocusMainWindow(App);

            // Start and attach to the TrafficSim application
            var trafficSimAppPath = GetApplicationPath("TrafficSim", "net6.0-windows", "TrafficSim.exe");
            AppSim = StartAndAttachToApplication(trafficSimAppPath);

            // Set up gRPC channel and client
            _channel = GrpcChannel.ForAddress($"http://localhost:{Test.Default.TestPort}");
            Client = new TestSim.TestSimClient(_channel);
        }

        [AfterScenario]
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

        public static void ClickButton(string automationId)
        {
            var mainWindow = SpecFlowHooks.App.GetMainWindow(SpecFlowHooks.Automation)
                            ?? throw new Exception("Main window could not be found.");

            // Find the button using its AutomationId
            var button = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsButton()
                         ?? throw new Exception($"Button with AutomationId '{automationId}' could not be found.");

            // Simulate button click
            button.Click();
        }

        /// <summary>
        /// Constructs the path to an application executable.
        /// </summary>
        private static string GetApplicationPath(string appName, string framework, string executable)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var workingDir = Path.Combine(baseDirectory, "..", "..", "..", "..", appName, "bin", "Debug", framework);
            var appPath = Path.Combine(workingDir, executable);

            if (!File.Exists(appPath))
            {
                throw new FileNotFoundException($"The application was not found at: {appPath}");
            }

            return appPath;
        }

        /// <summary>
        /// Starts and attaches to an application process using FlaUI.
        /// </summary>
        private static Application StartAndAttachToApplication(string appPath)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = appPath,
                WorkingDirectory = Path.GetDirectoryName(appPath),
                UseShellExecute = false
            };

            var process = Process.Start(processStartInfo) ?? throw new Exception("Failed to start the application.");
            return Application.Attach(process);
        }

        /// <summary>
        /// Focuses the main window of the application.
        /// </summary>
        private static void FocusMainWindow(Application app)
        {
            Automation = new UIA3Automation();
            var mainWindow = app.GetMainWindow(Automation) ?? throw new Exception("Main window could not be found.");
            mainWindow.Focus();
        }

        /// <summary>
        /// Closes the application gracefully and kills it if necessary.
        /// </summary>
        private static void CloseApplication(Application app, string appName)
        {
            if (app == null) return;

            try
            {
                app.Close();
                if (!app.HasExited)
                {
                    app.Kill();
                    Console.WriteLine($"{appName} application had to be forcibly terminated.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while closing {appName} application: {ex.Message}");
            }
        }

        /// <summary>
        /// Disposes of all shared resources.
        /// </summary>
        private static void CleanupResources()
        {
            Automation?.Dispose();
            App?.Dispose();
            AppSim?.Dispose();
            _channel?.Dispose();
        }
    }
}
