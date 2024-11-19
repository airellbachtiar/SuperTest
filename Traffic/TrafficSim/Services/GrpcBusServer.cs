using Bus;
using GrpcBusServer.Interfaces;
using GrpcBusServer.Services;
using GrpcHoster;
using PandISimComponents.Model;
using TrafficSim.Generated;

namespace TrafficSim.Services;

public class GrpcBusServer : IGrpcBus
{
    private readonly PandISimulator _fluidSimulator;
    private readonly Hoster _grpcHoster;

    public GrpcBusServer(
        PandISimulator fluidSimulator
        )
    {
        _fluidSimulator = fluidSimulator;

        _grpcHoster = new Hoster();
        var services = new GrpcServiceContainer();
        services.AddService<GrpcBusService, IGrpcBus>(Sim.Default.GrpcServerPort, this);
        _grpcHoster.HostServices(services.Services);
    }

    public void StopServer()
    {
        _grpcHoster.Stop();
    }

    public void SetPosition(PositionRequest request)
    {
        throw new NotImplementedException();
    }

    public PositionResponse GetPosition(Address address)
    {
        throw new NotImplementedException();
    }

    public void SetVelocity(VelocityRequest request)
    {
        throw new NotImplementedException();
    }

    public void Stop(Address address)
    {
        throw new NotImplementedException();
    }

    public void SetDigitalOutput(DigitalOutputRequest request)
    {
        var valve = _fluidSimulator.SimComponents
            .OfType<Valve>()
            .First(elm => elm.Id == request.Address.Id);
        valve.IsClosed = !request.Value;
    }

    public DigitalInputResponse GetDigitalInput(Address address)
    {
        throw new NotImplementedException();
    }

    public void SetPinIoModule(IoOutputRequest request)
    {
        throw new NotImplementedException();
    }

    public DigitalInputResponse GetPinIoModule(IoInputRequest input)
    {
        throw new NotImplementedException();
    }

    public void SetAnalogOutput(AnalogOutputRequest request)
    {
        throw new NotImplementedException();
    }

    public AnalogInputResponse GetAnalogInput(Address address)
    {
        throw new NotImplementedException();
    }

    public PumpResponse GetPumpState(Address address)
    {
        throw new NotImplementedException();
    }

    public void SetPumpState(PumpRequest request)
    {
        throw new NotImplementedException();
    }

    public VesselResponse GetVesselState(Address address)
    {
        throw new NotImplementedException();
    }

    public void SetVesselTemperature(VesselRequest request)
    {
        throw new NotImplementedException();
    }
}