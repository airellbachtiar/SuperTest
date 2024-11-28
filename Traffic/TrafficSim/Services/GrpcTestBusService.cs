using Grpc.Core;
using TestBus;

namespace TrafficSim.Services
{
    internal class GrpcTestBusService  : TestSim.TestSimBase
    {
        private readonly ITestSim _bus;

        public GrpcTestBusService(ITestSim bus)
        {
            _bus = bus;
        }

        public override Task<TestResponse> Test(Empty request, ServerCallContext context)
        {
            return Task.FromResult(_bus.Test());
        }

        public override Task<LightResponse> GetCarRedLightState(Empty request, ServerCallContext context)
        {
            return Task.FromResult(_bus.GetCarRedLightState());
        }

        public override Task<LightResponse> GetCarYellowLightState(Empty request, ServerCallContext context)
        {
            return Task.FromResult(_bus.GetCarYellowLightState());
        }

        public override Task<LightResponse> GetCarGreenLightState(Empty request, ServerCallContext context)
        {
            return Task.FromResult(_bus.GetCarGreenLightState());
        }

        public override Task<LightResponse> GetPedestrianRedLightState(Empty request, ServerCallContext context)
        {
            return Task.FromResult(_bus.GetPedestrianRedLightState());
        }

        public override Task<LightResponse> GetPedestrianGreenLightState(Empty request, ServerCallContext context)
        {
            return Task.FromResult(_bus.GetPedestrianGreenLightState());
        }

        public override Task<Empty> PressRequestPedestrianWalkButton(Empty request, ServerCallContext context)
        {
            _bus.PressRequestPedestrianWalkButton();
            return Task.FromResult(new Empty());
        }
    }
}
