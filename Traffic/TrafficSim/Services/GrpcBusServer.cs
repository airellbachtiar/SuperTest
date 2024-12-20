using Bus;
using Generated;
using GrpcBusServer.Interfaces;
using GrpcBusServer.Services;
using GrpcHoster;
using PandISimComponents.Model;
using SimulatorsBase.Interfaces;
using TrafficSim.Generated;

namespace TrafficSim.Services;

public class GrpcBusServer : IGrpcBus
{
    private readonly PandISimulator _fluidSimulator;
    private readonly MotionSimulator _motionSimulator;
    private readonly Hoster _grpcHoster;

    public GrpcBusServer(
        PandISimulator fluidSimulator,
        MotionSimulator motionSimulator
        )
    {
        _fluidSimulator = fluidSimulator;
        _motionSimulator = motionSimulator;

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
        var motionDevice = _motionSimulator.Loads[request.Address.Id];
        motionDevice.MoveTo(request.SetPoint, request.Speed, request.Acceleration, motionDevice.DefaultDeceleration);
    }

    public PositionResponse GetPosition(Address address)
    {
        var motionDevice = _motionSimulator.Loads[address.Id];
        return new PositionResponse
        {
            ActualPosition = motionDevice.Position,
            ActualSpeed = motionDevice.Velocity,
            Done = motionDevice.Done
        };
    }

    public void SetVelocity(VelocityRequest request)
    {
        var motionDevice = _motionSimulator.Loads[request.Address.Id];
        motionDevice.Move(request.Speed, request.Acceleration);
    }

    public void Stop(Address address)
    {
        var motionDevice = _motionSimulator.Loads[address.Id];
        motionDevice.Stop(motionDevice.DefaultDeceleration);
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
        if (_motionSimulator.DigitalSensors.TryGetValue(address.Id, out var digitalSensor))
        {
            return new DigitalInputResponse { Value = digitalSensor.Active };
        }

        if (_motionSimulator.DetectionSensors.TryGetValue(address.Id, out var sensor))
        {
            return new DigitalInputResponse { Value = sensor.Detected };
        }

        if (_motionSimulator.Switches.TryGetValue(address.Id, out var @switch))
        {
            return new DigitalInputResponse { Value = @switch.On };
        }

        var di = (ISimDigitalInput)_fluidSimulator.SimComponents
            .OfType<ElementBase>()
            .First(e => e.Id == address.Id && e is ISimDigitalInput);

        var response = new DigitalInputResponse
        {
            Value = di.GetValue()
        };
        return response;
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
        var simDevice = (ISimAnalogInput)_fluidSimulator.SimComponents
            .OfType<ElementBase>()
            .First(e => e is ISimAnalogInput && e.Id == address.Id);

        var response = new AnalogInputResponse
        {
            Value = simDevice.GetValue()
        };

        return response;
    }

    public PumpResponse GetPumpState(Address address)
    {
        var pump = _fluidSimulator.SimComponents
            .OfType<Pump>()
            .First(pump => pump.Id == address.Id);

        return new PumpResponse
        {
            ActualFlow = pump.PumpFlow
        };
    }

    public void SetPumpState(PumpRequest request)
    {
        var pump = _fluidSimulator.SimComponents
            .OfType<Pump>()
            .First(element => element.Id == request.Address.Id);

        pump.PumpFlow = request.Flow;
        if (request.On)
        {
            pump.On();
        }
        else
        {
            pump.Off();
        }
    }

    public VesselResponse GetVesselState(Address address)
    {
        var vessel = _fluidSimulator.SimComponents
            .OfType<Vessel>()
            .First(vessel => vessel.Id == address.Id);

        var pressure = vessel.GasPressure;
        var temperature = vessel.LiquidTemperatureCelsius;
        var volume = vessel.LitersLiquid;

        return new VesselResponse
        {
            ActualPressure = pressure,
            ActualTemperature = temperature,
            ActualVolume = volume
        };
    }

    public void SetVesselTemperature(VesselRequest request)
    {
        var vessel = _fluidSimulator.SimComponents
            .OfType<Vessel>()
            .First(vessel => vessel.Id == request.Address.Id);

        vessel.Temperature = request.Temperature;
    }
}