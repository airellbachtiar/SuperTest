using Logger;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StatemachineFramework.Logging;
using System.Configuration;
using System.Data;
using System.Windows;
using Traffic.Services;
using Traffic.ViewModel;

namespace Traffic
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IHost _host;

        protected override void OnStartup(StartupEventArgs e)
        {

            _host = new HostBuilder()
                .ConfigureAppConfiguration((_, _) =>
                {
                    // ConfigureServices
                }).ConfigureServices((_, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();

            _host.Start();
            base.OnStartup(e);
        }


        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var logger = _host.Services.GetRequiredService<ILogger>();
            var hmi = _host.Services.GetRequiredService<IDecompositionBuilder>().HMI;

            var viewModel = new MainWindowViewModel(hmi, logger)
            {
            };
            var mainWindow = new MainWindow(viewModel);
            mainWindow.Show();
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (_host)
            {
                var logger = _host.Services.GetRequiredService<IStatemachineLogger>();
                logger.Flush();
                var decompService = _host.Services.GetRequiredService<IDecompositionBuilder>();
                foreach (var hal in decompService.Hals)
                    await hal.DisposeKpiLoggerAsync();

                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging()
                .AddStatemachineServices()
                .AddBackgroundServices();
        }
    }

}
