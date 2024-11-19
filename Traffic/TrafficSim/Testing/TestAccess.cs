using TrafficSim.Services;

namespace TrafficSim.Testing
{
    public class TestAccess : ITestAccess
    {
        private readonly SimulatorService _simulatorService;

        public TestAccess(SimulatorService simulatorService)
        {
            _simulatorService = simulatorService;
        }

        public string GetCarRedCloseState()
        {
            return _simulatorService.FluidSimulator.CarRed.IsClosed ? "Close" : "Open";
        }

        public string GetPedestrianRedCloseState()
        {
            return _simulatorService.FluidSimulator.PedRed.IsClosed ? "Close" : "Open";
        }
    }
}
