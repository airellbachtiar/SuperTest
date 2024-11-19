
using Bus;
using GrpcHoster;
using TestBus;

namespace TrafficSim.Services
{
    public interface ITestSim : IRequiredService
    {
        TestResponse Test();
    }
}
