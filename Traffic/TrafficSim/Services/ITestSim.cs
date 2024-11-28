using GrpcHoster;
using TestBus;

namespace TrafficSim.Services
{
    public interface ITestSim : IRequiredService
    {
        TestResponse Test();
        LightResponse GetCarRedLightState();
        LightResponse GetCarYellowLightState();
        LightResponse GetCarGreenLightState();
        LightResponse GetPedestrianRedLightState();
        LightResponse GetPedestrianGreenLightState();
        void PressRequestPedestrianWalkButton();
    }
}
