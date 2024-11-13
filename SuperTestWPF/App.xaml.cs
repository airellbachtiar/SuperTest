using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperTestLibrary;
using SuperTestLibrary.Storages;
using SuperTestWPF.ViewModels;
using System.Windows;

namespace SuperTestWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public App()
        {
            this.Startup += OnStartup;
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<ISuperTestController, SuperTestController>();
                    services.AddSingleton<IReqIFStorage>(provider =>
                    {
                        var configuration = provider.GetRequiredService<IConfiguration>();
                        var gitFolderPath = configuration["LocalGitFolderPath"];
                        if (string.IsNullOrEmpty(gitFolderPath))
                        {
                            throw new InvalidOperationException("LocalGitFolderPath is not configured properly in appsettings.json.");
                        }
                        return new GitReqIFStorage(gitFolderPath);
                    });
                })
                .Build();
        }

        protected void OnStartup(object sender, StartupEventArgs e)
        {
            StartupAsync();
        }

        private async void StartupAsync()
        {
            await _host.StartAsync();
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }

}
