using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using TestBus;
using TrafficTest;
using TrafficTest.Models;

namespace TrafficTest
{
    [Binding]
    public class TrafficLightSystemSteps
    {
        private readonly TestSim.TestSimClient _client;

        public TrafficLightSystemSteps()
        {
            _client = TrafficTestHooks.Client;
        }

        [Given(@"the system is in idle state")]
        public void GivenTheSystemIsInIdleState()
        {
            TrafficTestHooks.ClickButton("StopButton");
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarYellow", "On");
        }

        [Given(@"the system is operational")]
        public void GivenTheSystemIsOperational()
        {
            TrafficTestHooks.ClickButton("StartButton");
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarGreen", "On");
        }

        [Given(@"the green traffic light is on")]
        public void GivenTheGreenTrafficLightIsOn()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarGreen", "On");
        }

        [Given(@"the yellow traffic light is on")]
        public void GivenTheYellowTrafficLightIsOn()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarYellow", "On");
        }

        [Given(@"the red traffic light is on")]
        public void GivenTheRedTrafficLightIsOn()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarRed", "On");
        }

        [When(@"the start button is pressed")]
        public void WhenTheStartButtonIsPressed()
        {
            TrafficTestHooks.ClickButton("StartButton");
        }

        [When(@"the pedestrian button is pressed")]
        public void WhenThePedestrianButtonIsPressed()
        {
            _client.PressRequestPedestrianWalkButton(new Empty());
        }

        [When(@"the yellow traffic light on time has elapsed")]
        public void WhenTheYellowTrafficLightOnTimeHasElapsed()
        {
            // Wait for the yellow light to turn off
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarYellow", "Off");
        }

        [When(@"the red traffic light on time has elapsed")]
        public void WhenTheRedTrafficLightOnTimeHasElapsed()
        {
            // Wait for the red light to turn off
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarRed", "Off");
        }

        [When(@"any traffic light is on")]
        public void WhenAnyTrafficLightIsOn()
        {
            // This step is already covered by the operational state
        }

        [Then(@"the system should transition to operational state")]
        public void ThenTheSystemShouldTransitionToOperationalState()
        {
            // Check if any of the car traffic lights are on
            var lastState = TrafficTestHooks.TrafficLightStates.Last();
            (lastState.CarGreen.LightState == "On" || lastState.CarYellow.LightState == "On" || lastState.CarRed.LightState == "On")
                .Should().BeTrue();
        }

        [Then(@"the green traffic light should be turned on")]
        public void ThenTheGreenTrafficLightShouldBeTurnedOn()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarGreen", "On");
        }

        [Then(@"the yellow traffic light should be turned on")]
        public void ThenTheYellowTrafficLightShouldBeTurnedOn()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarYellow", "On");
        }

        [Then(@"the red traffic light should be turned on")]
        public void ThenTheRedTrafficLightShouldBeTurnedOn()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarRed", "On");
        }

        [Then(@"only one traffic light should be on")]
        public void ThenOnlyOneTrafficLightShouldBeOn()
        {
            var lastState = TrafficTestHooks.TrafficLightStates.Last();
            int onLightsCount = new[]
            {
                lastState.CarGreen.LightState,
                lastState.CarYellow.LightState,
                lastState.CarRed.LightState
            }.Count(state => state == "On");

            onLightsCount.Should().Be(1);
        }

        [Then(@"the yellow traffic light should be blinking")]
        public void ThenTheYellowTrafficLightShouldBeBlinking()
        {
            var yellowLightStates = TrafficTestHooks.TrafficLightStates
                .Select(state => state.CarYellow.LightState)
                .ToList();

            // Check if the yellow light state changes at least once
            yellowLightStates.Distinct().Count().Should().BeGreaterThan(1);
        }
    }
}
