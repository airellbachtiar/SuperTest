using System;
using TechTalk.SpecFlow;
using FluentAssertions;
using TrafficTest;
using TrafficTest.Models;

namespace TrafficTest.Steps
{
    [Binding]
    [Scope(Feature = "System Synchronization")]
    public class SystemSynchronizationSteps
    {
        [Given(@"the system is operational")]
        public void GivenTheSystemIsOperational()
        {
            TrafficTestHooks.ClickButton("StartButton");
        }

        [When(@"the traffic light is green or yellow")]
        public void WhenTheTrafficLightIsGreenOrYellow()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarGreen", "On");
        }

        [Then(@"the pedestrian light should be red")]
        public void ThenThePedestrianLightShouldBeRed()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("PedestrianRed", "On");
            var lastState = TrafficTestHooks.TrafficLightStates.LastOrDefault();
            lastState.Should().NotBeNull();
            lastState.PedestrianRed.LightState.Should().Be("On");
        }

        [Given(@"the traffic light is red")]
        public void GivenTheTrafficLightIsRed()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarRed", "On");
        }

        [When(@"the pedestrian button has been pressed")]
        public void WhenThePedestrianButtonHasBeenPressed()
        {
            TrafficTestHooks.Client.PressRequestPedestrianWalkButton(new TestBus.Empty());
        }

        [Then(@"the pedestrian light should turn green")]
        public void ThenThePedestrianLightShouldTurnGreen()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("PedestrianGreen", "On");
            var lastState = TrafficTestHooks.TrafficLightStates.LastOrDefault();
            lastState.Should().NotBeNull();
            lastState.PedestrianGreen.LightState.Should().Be("On");
        }

        [Given(@"the pedestrian light is green")]
        public void GivenThePedestrianLightIsGreen()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("PedestrianGreen", "On");
        }

        [Then(@"the traffic light should be red")]
        public void ThenTheTrafficLightShouldBeRed()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarRed", "On");
            var lastState = TrafficTestHooks.TrafficLightStates.LastOrDefault();
            lastState.Should().NotBeNull();
            lastState.CarRed.LightState.Should().Be("On");
        }
    }
}
