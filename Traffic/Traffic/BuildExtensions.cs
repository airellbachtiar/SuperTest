using Logger;
using Microsoft.Extensions.DependencyInjection;
using StatemachineFramework.Logging;
using StatemachineFramework.Loggers.StateViewer.Binary;
using Traffic.Services;
using Traffic.HMI;
using Traffic.Logger;
using StatemachineFramework.Loggers;


namespace Traffic;

public static class BuildExtensions
{
    public static IServiceCollection AddLogging(this IServiceCollection services)
    {
        services.AddSingleton<ILogger, LogViewModel>();
        services.AddSingleton<IStatemachineLogger, CompoundLogger>(x => new CompoundLogger(
                new LogViewLogger(x.GetRequiredService<ILogger>()),
                new BinaryStatemachineLogger(@"..\..\..\Logs\StateViewerDefinition.json",
                    storageDirectory: @"..\..\..\Logs",
                    minutesBeforeSave: 1),
                new StateToHmiLogger(x))
        );
        return services;
    }

    public static IServiceCollection AddStatemachineServices(this IServiceCollection services)
    {
        services.AddSingleton<IDecompositionBuilder, DecompositionBuilderService>();
        return services;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<StatemachineRunnerService>();
        services.AddHostedService<CommunicationService>();
        return services;
    }
}