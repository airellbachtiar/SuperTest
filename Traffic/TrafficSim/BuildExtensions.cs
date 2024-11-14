using TrafficSim.Generated;
using Logger;
using Microsoft.Extensions.DependencyInjection;

using TrafficSim.Logger;
using TrafficSim.Services;
using TrafficSim.Testing;

namespace TrafficSim;

public static class BuildExtensions
{
    public static IServiceCollection AddLogging(this IServiceCollection services)
    {
        services.AddSingleton<ILogger, LogViewModel>();
        return services;
    }

    public static IServiceCollection AddSimulators(this IServiceCollection services)
    {
        services.AddSingleton<PandISimulator>();
        services.AddSingleton<HolodeckFluidStateSyncService>();
        services.AddSingleton<SimulatorService>();
        services.AddSingleton<TestAccess>();
        return services;
    }
}