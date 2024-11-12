using TrafficSim.Generated;
using SimulatorsBase;
using System;
using Resources.Fluids;
using HolodeckGrpcServer.Interfaces;
using HolodeckGrpcServer;
using Logger;
using GrpcHoster;
using HolodeckGrpcServer.Services;

namespace TrafficSim.Services;

public class SimulatorService : SimulatorBase, IHoloSource
{
    public event EventHandler<HolodeckEventArgs>? MessageToHolodeck;
    public event EventHandler<HolodeckEventArgs>? MessageFromHolodeck;
    public event EventHandler? StopListeners;

    private readonly TrafficSim.Services.GrpcBusServer _grpcBus;
    private readonly ILogger _logger;
    private readonly Hoster _hoster;
    private HolodeckFluidStateSyncService _interlockService;

    public SimulatorService(
        PandISimulator fluidSimulator,
        HolodeckFluidStateSyncService interlockService,
        ILogger logger)
    {
        FluidSimulator = fluidSimulator;
        _interlockService = interlockService;

        _logger = logger;

        FluidDatabase.InitializeDatabase("fluids.xml");
        _grpcBus = new TrafficSim.Services.GrpcBusServer(fluidSimulator);

        _hoster = new Hoster();
        var services = new GrpcServiceContainer();
        services.AddService<HolodeckService, IHoloSource>(Sim.Default.HOLODECK_PORT, this);
        _hoster.HostServices(services.Services);
    }

    public override TimeSettings TimeStepSettings { get; set; } = new()
    {
        // Change to 0.1 time step to go 100 x RT
        MinimumStep = TimeSpan.FromSeconds(0.01),
        NominalStep = TimeSpan.FromSeconds(0.01)
    };

    public PandISimulator FluidSimulator { get; }

    protected override void Cycle(TimeSettings timeSettings)
    {
        FluidSimulator.Calculate(timeSettings);
        _interlockService.Cycle();
    }

    public override void Stop()
    {
        _grpcBus.StopServer();
        StopListeners?.Invoke(this, EventArgs.Empty);
        base.Stop();
    }

    public ILogger GetLogger()
    {
        return _logger;
    }

    public IHoloMotion GetMotion()
    {
        return null;
    }
}