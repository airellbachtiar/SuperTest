using FlaUI.Core;
using FlaUI.UIA3;
using System.Diagnostics;

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
