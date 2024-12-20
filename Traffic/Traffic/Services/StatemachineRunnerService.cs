using HalFramework;
using InterfaceServices.Model;
using Logger;
using Microsoft.Extensions.Hosting;
using StatemachineFramework.Components;
using StatemachineFramework.Runner;

namespace Traffic.Services;

public class StatemachineRunnerService : BackgroundService
{
    private readonly IStatemachineRunner _runner;
    private readonly IEnumerable<Component> _components;
    private readonly IEnumerable<IBaseInterface> _interfaces;
    private readonly IEnumerable<Hal> _hals;
    private readonly ILogger _logger;

    public StatemachineRunnerService(
        IDecompositionBuilder builder,
        ILogger logger)
    {
        _components = builder.Components;
        _interfaces = builder.Interfaces;
        _hals = builder.Hals;
        _runner = new SingleCycleStatemachineRunner(_components.ToList());
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Run();
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, "Encountered error while running the state machine", e);
                    // Run();
                }
            }
        }, stoppingToken);
    }


    private void Run()
    {
        foreach (var component in _components)
        {
            component.InBuffer.Process();
            component.Run();
        }

        foreach (var @interface in _interfaces)
        {
            @interface.UpdateProperties();
        }

        foreach (var hal in _hals)
        {
            hal.Process();
            hal.ReadWrite();
            hal.Run();
        }

        _runner.Run();
    }
}