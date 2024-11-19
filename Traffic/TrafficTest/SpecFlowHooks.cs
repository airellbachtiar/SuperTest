using FlaUI.Core;
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
        public static Application AppSim { get; private set; }
        public static UIA3Automation Automation { get; private set; }

        public static TestSim.TestSimClient Client { get; private set; }
        private static GrpcChannel _channel;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var workingDir = Path.Combine(baseDirectory, "..", "..", "..", "..", "Traffic", "bin", "Debug", "net8.0-windows");
            var appPath = Path.Combine(workingDir, "Traffic.exe");

            if (!File.Exists(appPath))
            {
                throw new FileNotFoundException($"The application was not found at: {appPath}");
            }

            //Run the application (using FlaUI not working)
            var processStartInfo = new ProcessStartInfo
            {
                FileName = appPath,
                WorkingDirectory = workingDir,
                UseShellExecute = false
            };

            var process = Process.Start(processStartInfo) ?? throw new Exception("Failed to start the application.");

            // Attach to the process using FlaUI
            App = Application.Attach(process);
            Automation = new UIA3Automation();
            var mainWindow = App.GetMainWindow(Automation) ?? throw new Exception("Main window could not be found.");
            mainWindow.Focus();

            // Run TrafficSim
            var workingDirSim = Path.Combine(baseDirectory, "..", "..", "..", "..", "TrafficSim", "bin", "Debug", "net6.0-windows");
            var appPathSim = Path.Combine(workingDirSim, "TrafficSim.exe");

            if (!File.Exists(appPathSim))
            {
                throw new FileNotFoundException($"The application was not found at: {appPathSim}");
            }

            //Run the application (using FlaUI not working)
            var processStartInfoSim = new ProcessStartInfo
            {
                FileName = appPathSim,
                WorkingDirectory = workingDirSim,
                UseShellExecute = false
            };

            var processSim = Process.Start(processStartInfoSim) ?? throw new Exception("Failed to start the application.");
            AppSim = Application.Attach(processSim);

            _channel = GrpcChannel.ForAddress($"http://localhost:{Test.Default.TestPort}");
            Client = new TestSim.TestSimClient(_channel);
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            try
            {
                App?.Close();
                AppSim?.Close();
                if (!App.HasExited)
                {
                    App.Kill();
                }

                if (!AppSim.HasExited)
                {
                    AppSim.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while closing the application: {ex.Message}");
            }
            finally
            {
                Automation?.Dispose();
                App?.Dispose();
                AppSim?.Dispose();
                _channel?.Dispose();
            }
        }
    }
}
