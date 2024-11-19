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

        public override Task<TestResponse> Test(TestBus.Empty request, ServerCallContext context)
        {
            return Task.FromResult(_bus.Test());
        }

        public override Task<LightResponse> GetCarRedLightState(TestBus.Empty request, ServerCallContext context)
        {
            return Task.FromResult(_bus.GetCarRedLightState());
        }

        public override Task<LightResponse> GetCarYellowLightState(TestBus.Empty request, ServerCallContext context)
        {
            return Task.FromResult(_bus.GetCarYellowLightState());
        }

        public override Task<LightResponse> GetCarGreenLightState(TestBus.Empty request, ServerCallContext context)
        {
            return Task.FromResult(_bus.GetCarGreenLightState());
        }

        public override Task<LightResponse> GetPedestrianRedLightState(TestBus.Empty request, ServerCallContext context)
        {
            return Task.FromResult(_bus.GetPedestrianRedLightState());
        }

        public override Task<LightResponse> GetPedestrianGreenLightState(TestBus.Empty request, ServerCallContext context)
        {
            return Task.FromResult(_bus.GetPedestrianGreenLightState());
        }
    }
}
