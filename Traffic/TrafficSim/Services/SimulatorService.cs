using TrafficSim.Generated;
using SimulatorsBase;
using Resources.Fluids;
using HolodeckGrpcServer.Interfaces;
using HolodeckGrpcServer;
using Logger;
using GrpcHoster;
using HolodeckGrpcServer.Services;
using TestBus;
using Generated;

namespace TrafficSim.Services;

public class SimulatorService : SimulatorBase, IHoloSource, ITestSim
{
    public event EventHandler<HolodeckEventArgs>? MessageToHolodeck;
    public event EventHandler<HolodeckEventArgs>? MessageFromHolodeck;
    public event EventHandler? StopListeners;

    private readonly GrpcBusServer _grpcBus;
    private readonly ILogger _logger;
    private readonly Hoster _hoster;
    private HolodeckFluidStateSyncService _interlockService;

    public SimulatorService(
        PandISimulator fluidSimulator,
        MotionSimulator motionSimulator,
        HolodeckFluidStateSyncService interlockService,
        ILogger logger)
    {
        FluidSimulator = fluidSimulator;
        MotionSimulator = motionSimulator;
        _interlockService = interlockService;

        _logger = logger;

        FluidDatabase.InitializeDatabase("fluids.xml");
        _grpcBus = new GrpcBusServer(fluidSimulator, motionSimulator);

        _hoster = new Hoster();
        var services = new GrpcServiceContainer();
        services.AddService<HolodeckService, IHoloSource>(Sim.Default.HOLODECK_PORT, this);
        services.AddService<GrpcTestBusService, ITestSim>(Sim.Default.TestPort, this);
        _hoster.HostServices(services.Services);
    }

    public override TimeSettings TimeStepSettings { get; set; } = new()
    {
        // Change to 0.1 time step to go 100 x RT
        MinimumStep = TimeSpan.FromSeconds(0.01),
        NominalStep = TimeSpan.FromSeconds(0.01)
    };

    public PandISimulator FluidSimulator { get; }
    public MotionSimulator MotionSimulator { get; }

    protected override void Cycle(TimeSettings timeSettings)
    {
        MotionSimulator.SimCycle(timeSettings.NominalStep);
        FluidSimulator.Calculate(timeSettings);
        _interlockService.Cycle();

        MessageToHolodeck?.Invoke(this, new HolodeckEventArgs(MotionSimulator));
        MessageFromHolodeck?.Invoke(this, new HolodeckEventArgs(MotionSimulator));
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
        return MotionSimulator;
    }

    public TestResponse Test()
    {
        var x = new TestResponse
        {
            ResponseCode = 42
        };
        return x;
    }

    public LightResponse GetCarRedLightState()
    {
        return LightResponseBuilder(FluidSimulator.CarRed.ElementName, FluidSimulator.CarRed.IsClosed);
    }

    public LightResponse GetCarYellowLightState()
    {
        return LightResponseBuilder(FluidSimulator.CarYellow.ElementName, FluidSimulator.CarYellow.IsClosed);
    }

    public LightResponse GetCarGreenLightState()
    {
        return LightResponseBuilder(FluidSimulator.CarGreen.ElementName, FluidSimulator.CarGreen.IsClosed);
    }

    public LightResponse GetPedestrianRedLightState()
    {
        return LightResponseBuilder(FluidSimulator.PedRed.ElementName, FluidSimulator.PedRed.IsClosed);
    }

    public LightResponse GetPedestrianGreenLightState()
    {
        return LightResponseBuilder(FluidSimulator.PedGreen.ElementName, FluidSimulator.PedGreen.IsClosed);
    }

    private LightResponse LightResponseBuilder(string name, bool state)
    {
        var valveResponse = new LightResponse
        {
            LightName = name,
            LightState = state ? "Off" : "On"
        };
        return valveResponse;
    }
}