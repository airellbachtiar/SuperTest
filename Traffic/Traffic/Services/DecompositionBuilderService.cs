using Comm.Grpc;
using HalFramework;
using InterfaceServices.Model;
using StatemachineFramework.Components;
using StatemachineFramework.Logging;
using Traffic.Generated;
using Traffic.Generated.HAL1;
// ReSharper disable InconsistentNaming

namespace Traffic.Services;

public interface IDecompositionBuilder
{
    public IEnumerable<Component> Components { get; }
    public IEnumerable<IBaseInterface> Interfaces { get; }
    public IEnumerable<Hal> Hals { get; }
    public IEnumerable<ICommunicator> CommunicationElements { get; }
    public Generated.HMI.HMI HMI { get; }
}

/// <summary>
/// Most likely instantiated using dependency injection
/// </summary>
public class DecompositionBuilderService : IDecompositionBuilder
{
    private readonly Factory _factory;
    private readonly HAL1Communication _grpcHal;
    private readonly IStatemachineLogger _logger;

    public DecompositionBuilderService(
        IStatemachineLogger logger)
    {
        _factory = new Factory();
        _grpcHal = _factory.TrafficDomainModelHal.HAL1Communication;
        _logger = logger;

        ConfigureBus();
        ConfigureCommunication();

        _factory.Build();
        _factory.TrafficDomainModel.HMI.Components = Components;

        AddLoggers();
    }

    public Generated.HMI.HMI HMI => _factory.TrafficDomainModel.HMI;
    public IEnumerable<Component> Components => _factory.TrafficDomainModel.Components;
    public IEnumerable<IBaseInterface> Interfaces => _factory.TrafficDomainModel.Interfaces;
    public IEnumerable<Hal> Hals => _factory.TrafficDomainModelHal.Hals;
    public IEnumerable<ICommunicator> CommunicationElements =>
        _factory.TrafficDomainModelHal.HAL1Communication.CommunicationElements;

    private void ConfigureBus()
    {
        // host and port should be defined as configuration parameters
        const string host = "http://localhost";
        const int port = 3456;

        _grpcHal.GrpcBusClient = new GrpcBusClient();
        _grpcHal.GrpcBusClient.Connect(host, port);
    }

    private static void ConfigureCommunication()
    {
        // Intentionally left blank for now.
    }

    private void AddLoggers()
    {
        AddComponentLogging();
        AddHalLogging();
    }

    private void AddComponentLogging()
    {
        foreach (var component in Components)
        {
            component.AddLogger(_logger);
        }
    }

    private void AddHalLogging() { }
}