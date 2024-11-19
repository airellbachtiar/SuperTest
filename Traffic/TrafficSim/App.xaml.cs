using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Windows;
using TrafficSim.Services;
using TrafficSim.ViewModel;

namespace TrafficSim
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {
        private readonly IHost _host;

        public App()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            _host = new HostBuilder()
                .ConfigureAppConfiguration((_, _) =>
                {
                    // Nothing to configure for now
                })
                .ConfigureServices((_, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            var simulatorService = _host.Services.GetRequiredService<SimulatorService>();

            var viewModel = new MainWindowViewModel(simulatorService);
            var mainWindow = new MainWindow(viewModel);
            mainWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging()
                .AddSimulators();
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
        }

    }

}
