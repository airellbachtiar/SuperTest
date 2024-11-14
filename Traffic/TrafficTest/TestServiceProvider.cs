using Microsoft.Extensions.DependencyInjection;
using TrafficSim.Generated;
using TrafficSim.Services;
using TrafficSim.Testing;

using Logger;
using TrafficSim.Logger;

namespace TrafficTest
{
    public static class TestServiceProvider
    {
        public static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ILogger, LogViewModel>();

            // Register TrafficSim services
            services.AddSingleton<PandISimulator>();
            services.AddSingleton<HolodeckFluidStateSyncService>();
            services.AddSingleton<SimulatorService>();
            services.AddSingleton<ITestAccess, TestAccess>();

            // Add other necessary services for TrafficTest

            return services.BuildServiceProvider();
        }
    }
}
