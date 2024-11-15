using FlaUI.Core;
using FlaUI.UIA3;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using TrafficSim.Testing;

namespace TrafficTest
{
    [Binding]
    public class SpecFlowHooks
    {
        private static ServiceProvider _serviceProvider;
        public static ITestAccess TestAccess { get; private set; }

        public static Application App { get; private set; }
        public static UIA3Automation Automation { get; private set; }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            // Set up the service provider and register TestAccess
            _serviceProvider = TestServiceProvider.ConfigureServices();
            TestAccess = _serviceProvider.GetService<ITestAccess>();

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
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            try
            {
                App?.Close();
                if (!App.HasExited)
                {
                    App.Kill();
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
                _serviceProvider.Dispose();
            }
        }
    }
}
