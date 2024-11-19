using TrafficSim.Generated;
using TrafficSim.Services;

namespace TrafficSim.ViewModel;

public class MainWindowViewModel : IDisposable
{
    private readonly SimulatorService _simulatorService;
    private bool _disposed;

    public MainWindowViewModel(SimulatorService simulatorService)
    {
        _simulatorService = simulatorService;
        _simulatorService.Start();
    }

    public PandISimulator FluidSimulator => _simulatorService.FluidSimulator;

    #region IDispose

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _simulatorService.Stop();
        }

        _disposed = true;
    }

    #endregion
}