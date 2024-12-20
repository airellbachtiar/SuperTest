using HalFramework;
using Microsoft.Extensions.Hosting;

namespace Traffic.Services;

public class CommunicationService : BackgroundService
{
    private readonly IEnumerable<ICommunicator> _communicationElements;

    public CommunicationService(IDecompositionBuilder builder)
    {
        _communicationElements = builder.CommunicationElements;
    }

    private void Run()
    {
        foreach (var communicationElements in _communicationElements)
        {
            communicationElements.Run();
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Run();
            }
        }, stoppingToken);
    }
}