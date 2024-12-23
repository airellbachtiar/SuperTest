using FlaUI.Core;
using FlaUI.UIA3;
using Grpc.Net.Client;
using System.Diagnostics;
using TestBus;
using TrafficTest.Models;

namespace TrafficTest
{
    public partial class TrafficTestHooks
    {
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
            _automation = new UIA3Automation();
            var mainWindow = app.GetMainWindow(_automation) ?? throw new Exception("Main window could not be found.");
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
            _automation?.Dispose();
            _app?.Dispose();
            _appSim?.Dispose();
            _channel?.Dispose();
        }

        private static void InitializeTrafficApplication()
        {
            var trafficAppPath = GetApplicationPath("Traffic", "net8.0-windows", "Traffic.exe");
            _app = StartAndAttachToApplication(trafficAppPath);
            FocusMainWindow(_app);
        }

        private static void InitializeTrafficSimulationApplication()
        {
            var trafficSimAppPath = GetApplicationPath("TrafficSim", "net6.0-windows", "TrafficSim.exe");
            _appSim = StartAndAttachToApplication(trafficSimAppPath);
            Task.Delay(2000).Wait(); // Allow applications to load
        }

        private static void InitializeGrpcConnection()
        {
            _channel = GrpcChannel.ForAddress($"http://localhost:{Test.Default.TestPort}");
            Client = new TestSim.TestSimClient(_channel);
        }

        private static void InitializeStateTracking()
        {
            TrafficLightStates = new List<TrafficLightState>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private static void StartTrafficLightObservation()
        {
            Task.Run(() => ObserveTrafficLight(_cancellationTokenSource!.Token));
        }

        private static void CleanupScenarioResources()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            Task.Delay(1000).Wait();
        }

        private static void CleanupTestEnvironment()
        {
            try
            {
                CloseApplication(_app, "Traffic");
                CloseApplication(_appSim, "TrafficSim");
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
    }
}
