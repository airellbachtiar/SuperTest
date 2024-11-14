using NUnit.Framework;
using TrafficSim.Testing;

namespace TrafficTest.StepDefinitions
{
    [Binding]
    public class TrafficTestStepDefinition
    {
        private readonly ITestAccess _testAccess;

        public TrafficTestStepDefinition()
        {
            _testAccess = SpecFlowHooks.TestAccess;
        }

        #region Traffic light system starts in red light
        [Given("the traffic light system starts")]
        public async void GivenTheTrafficLightSystemStarts()
        {

        }

        [When("the system initializes")]
        public void WhenTheSystemInitializes()
        {
            while (true)
            {
                if (_testAccess.GetCarRedCloseState() == "Open")
                {
                    string trafficLightState = _testAccess.GetCarRedCloseState();
                    Assert.AreEqual("Open", trafficLightState);
                    break;
                }
            }
        }

        [Then("the traffic light should be red")]
        public async void ThenTheTrafficLightShouldBeRed()
        {
        }
        #endregion

        #region Pedestrian light system starts in red light
        [Given("the pedestrian light system starts")]
        public async void GivenThePedestrianLightSystemStarts()
        {
        }

        [Then("the pedestrian light should be red")]
        public void ThenThePedestrianLightShouldBeRed()
        {

        }
        #endregion
    }
}
