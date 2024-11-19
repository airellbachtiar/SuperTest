using System.Threading.Tasks;
using Bus;
using Grpc.Core;
using GrpcBusServer.Interfaces;
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
    }
}
