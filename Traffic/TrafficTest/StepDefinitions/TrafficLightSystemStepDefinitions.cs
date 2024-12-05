using System;
using TechTalk.SpecFlow;
using FluentAssertions;
using TestBus;

namespace TrafficTest.Steps
{
    [Binding]
    [Scope(Feature = "Traffic Light System")]
    public class TrafficLightSystemSteps
    {
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
            TrafficTestHooks.Client.PressRequestPedestrianWalkButton(new Empty());
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
            // This step is implicitly covered by other steps
        }

        [Then(@"the system should transition to operational state")]
        public void ThenTheSystemShouldTransitionToOperationalState()
        {
            // Verify that the green light is on, indicating operational state
            TrafficTestHooks.WaitUntilTheLightIsInThatState("CarGreen", "On");
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
            var lastState = TrafficTestHooks.TrafficLightStates.LastOrDefault();
            lastState.Should().NotBeNull();

            int onLightsCount = 0;
            if (lastState.CarRed.LightState == "On") onLightsCount++;
            if (lastState.CarYellow.LightState == "On") onLightsCount++;
            if (lastState.CarGreen.LightState == "On") onLightsCount++;

            onLightsCount.Should().Be(1, "Only one traffic light should be on at a time");
        }

        [Then(@"the yellow traffic light should be blinking")]
        public void ThenTheYellowTrafficLightShouldBeBlinking()
        {
            // Observe the yellow light state for a period of time
            var startTime = DateTime.Now;
            var observationPeriod = TimeSpan.FromSeconds(10);
            bool hasBeenOn = false;
            bool hasBeenOff = false;

            while (DateTime.Now - startTime < observationPeriod)
            {
                var currentState = TrafficTestHooks.TrafficLightStates.LastOrDefault();
                if (currentState != null)
                {
                    if (currentState.CarYellow.LightState == "On") hasBeenOn = true;
                    if (currentState.CarYellow.LightState == "Off") hasBeenOff = true;

                    if (hasBeenOn && hasBeenOff) break;
                }
                System.Threading.Thread.Sleep(100);
            }

            hasBeenOn.Should().BeTrue("The yellow light should have been on during the observation period");
            hasBeenOff.Should().BeTrue("The yellow light should have been off during the observation period");
        }
    }
}
