using NUnit.Framework;
using TestBus;

namespace TrafficTest.Steps
{
    [Binding]
    [Scope(Feature = "Traffic Light System")]
    [NonParallelizable]
    public class TrafficLightSteps
    {
        private readonly TestSim.TestSimClient _client;

        public TrafficLightSteps()
        {
            _client = SpecFlowHooks.Client;
        }

        [Given(@"the traffic light system is off")]
        public void GivenTheTrafficLightSystemIsOff()
        {
            // No need to do anything here
        }

        [Given(@"the traffic light system is not started")]
        public void GivenTheTrafficLightSystemIsNotStarted()
        {
            // No need to do anything here
        }

        [Given(@"the traffic light system has started")]
        public void GivenTheTrafficLightSystemHasStarted()
        {
            WhenTheUserStartsTheTrafficLightSystem();
        }

        [When(@"the user presses the start button")]
        public void WhenTheUserPressesTheStartButton()
        {
            SpecFlowHooks.ClickButton("StartButton");
        }

        [When(@"the user starts the traffic light system")]
        public void WhenTheUserStartsTheTrafficLightSystem()
        {
            WhenTheUserPressesTheStartButton();
            Thread.Sleep(1000);
        }

        [When(@"(\d+) second(?:s)? \b(?:has|have)\b passed")]
        public void WhenSecondsHasPassed(int seconds)
        {
            _client.PressRequestPedestrianWalkButton(new Empty());
            SpecFlowHooks.ObserveTrafficLight(seconds * 1000);
        }

        [Then(@"the traffic light system should start")]
        public async Task ThenTheTrafficLightSystemShouldStart()
        {
            Thread.Sleep(200);
            var response = await _client.GetCarGreenLightStateAsync(new Empty());
            response.LightState.Should().Be("On");
        }

        [Then(@"the car's yellow light should be blinking")]
        public void ThenTheCarsYellowLightShouldBeBlinking()
        {
            SpecFlowHooks.ObserveTrafficLight(2000);
            SpecFlowHooks.LightResponses.Should().Contain(x => x.LightState == "On" && x.LightName == "CarYellow");
        }

        [Then(@"the pedestrian green light should be blinking")]
        public void ThenThePedestrianGreenLightShouldBeBlinking()
        {
            SpecFlowHooks.ObserveTrafficLight(2000);
            SpecFlowHooks.LightResponses.Should().Contain(x => x.LightState == "On" && x.LightName == "PedGreen");
        }

        [Then(@"the car's green light should turn on")]
        public void ThenTheCarSGreenLightShouldTurnOn()
        {
            var response = _client.GetCarGreenLightState(new Empty());
            response.LightState.Should().Be("On");
        }

        [Then(@"the pedestrian red light should turn on")]
        public void  ThenThePedestrianRedLightShouldTurnOn()
        {
            var response = _client.GetPedestrianRedLightState(new Empty());
            response.LightState.Should().Be("On");
        }

        [Then(@"the car's green light should switch to yellow")]
        public void ThenTheCarSGreenLightShouldSwitchToYellow()
        {
            SpecFlowHooks.LightResponses.Should().Contain(new LightResponse { LightName = "CarYellow", LightState = "On" });
        }

        [Then(@"the pedestrian red light should remain on")]
        public void ThenThePedestrianRedLightShouldRemainOn()
        {
            SpecFlowHooks.LightResponses.Should().Contain(new LightResponse { LightName = "PedRed", LightState = "On" });
        }

        [Then(@"the car's yellow light should switch to red")]
        public void ThenTheCarSYellowLightShouldSwitchToRed()
        {
            SpecFlowHooks.LightResponses.Should().Contain(new LightResponse { LightName = "CarRed", LightState = "On" });
        }

        [Then(@"the pedestrian green light should turn on")]
        public void ThenThePedestrianGreenLightShouldTurnOn()
        {
            SpecFlowHooks.LightResponses.Should().Contain(new LightResponse { LightName = "PedGreen", LightState = "On" });
        }

        [Then(@"the pedestrian light should switch to red")]
        public void ThenThePedestrianLightShouldSwitchToRed()
        {
            SpecFlowHooks.LightResponses.Should().Contain(new LightResponse { LightName = "PedRed", LightState="On"});
        }

        [Then(@"the car green light should turn back on")]
        public void ThenTheCarGreenLightShouldTurnBackOn()
        {
            var last10 = SpecFlowHooks.LightResponses.Skip(Math.Max(0, SpecFlowHooks.LightResponses.Count() - 10)).ToList();
            last10.Should().Contain(new LightResponse { LightName = "CarGreen", LightState = "On"});
        }
    }
}
