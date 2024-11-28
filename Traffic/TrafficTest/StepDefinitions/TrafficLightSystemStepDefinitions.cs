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
            _client = TrafficTestHooks.Client;
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
            TrafficTestHooks.ClickButton("StartButton");
        }

        [When(@"the user presses the start button")]
        public void WhenTheUserPressesTheStartButton()
        {
            TrafficTestHooks.ClickButton("StartButton");
        }

        [When(@"the user starts the traffic light system")]
        public void WhenTheUserStartsTheTrafficLightSystem()
        {
            TrafficTestHooks.ClickButton("StartButton");
        }

        [When(@"(\d+) second(?:s)? \b(?:has|have)\b passed")]
        public void WhenSecondsHasPassed(int seconds)
        {
            _client.PressRequestPedestrianWalkButton(new Empty());
            Task.Delay(seconds * 1000).Wait();
        }

        [Then(@"the car's yellow light should be blinking")]
        public void ThenTheCarsYellowLightShouldBeBlinking()
        {
            Thread.Sleep(2000);
            var carYellowLight = TrafficTestHooks.TrafficLightStates
                .Select(x => x.CarYellow)
                .ToList();

            carYellowLight.Should().Contain(x => x.LightState == "On");
            carYellowLight.Should().Contain(x => x.LightState == "Off");
        }

        [Then(@"the pedestrian green light should be blinking")]
        public void ThenThePedestrianGreenLightShouldBeBlinking()
        {
            Thread.Sleep(2000);
            var pedestrianGreenLight = TrafficTestHooks.TrafficLightStates
                .Select(x => x.PedestrianGreen)
                .ToList();

            pedestrianGreenLight.Should().Contain(x => x.LightState == "On");
            pedestrianGreenLight.Should().Contain(x => x.LightState == "Off");
        }

        [Then(@"the car's green light should switch to yellow")]
        public void ThenTheCarSGreenLightShouldSwitchToYellow()
        {
            TrafficTestHooks.TrafficLightStates
                .Select(x => x.CarYellow)
                .Last()
                .LightState.Should().Be("On");
        }

        [Then(@"the pedestrian red light should remain on")]
        public void ThenThePedestrianRedLightShouldRemainOn()
        {
            TrafficTestHooks.TrafficLightStates
                .Select(x => x.PedestrianRed)
                .Last()
                .LightState.Should().Be("On");
        }

        [Then(@"the car's yellow light should switch to red")]
        public void ThenTheCarSYellowLightShouldSwitchToRed()
        {
            TrafficTestHooks.TrafficLightStates
                .Select(x => x.CarRed)
                .Last()
                .LightState.Should().Be("On");
        }

        [Then(@"the pedestrian green light should turn on")]
        public void ThenThePedestrianGreenLightShouldTurnOn()
        {
            TrafficTestHooks.TrafficLightStates
                .Select(x => x.PedestrianGreen)
                .Last()
                .LightState.Should().Be("On");
        }

        [Then(@"the pedestrian light should switch to red")]
        [Then(@"the pedestrian red light should turn on")]
        public void ThenThePedestrianLightShouldSwitchToRed()
        {
            TrafficTestHooks.TrafficLightStates
                .Select(x => x.PedestrianRed)
                .Last()
                .LightState.Should().Be("On");
        }

        [Then(@"the car green light should turn back on")]
        [Then(@"the car's green light should turn on")]
        [Then(@"the traffic light system should start")]
        public void ThenTheCarGreenLightShouldTurnBackOn()
        {
            TrafficTestHooks.TrafficLightStates
                .Select(x => x.CarGreen)
                .Last()
                .LightState.Should().Be("On");
        }
    }
}
