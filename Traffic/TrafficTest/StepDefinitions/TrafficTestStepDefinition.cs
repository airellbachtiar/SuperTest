namespace TrafficTest.StepDefinitions
{
    [Binding]
    public class TrafficTestStepDefinition
    {

        #region Traffic light system starts in red light
        [Given("the traffic light system starts")]
        public async void GivenTheTrafficLightSystemStarts()
        {
        }

        [When("the system initializes")]
        public void WhenTheSystemInitializes()
        {
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
