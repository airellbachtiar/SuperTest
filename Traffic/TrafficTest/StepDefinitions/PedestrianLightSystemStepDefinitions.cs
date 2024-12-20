using System;
using NUnit.Framework;
using FluentAssertions;
using TechTalk.SpecFlow;
using TrafficTest;

namespace TrafficTest.Steps
{
    [Binding]
    [Scope(Feature = "Pedestrian Light System")]
    public class PedestrianLightSystemSteps
    {
        [Given(@"the system is in idle state")]
        public void GivenTheSystemIsInIdleState()
        {
            // Assuming the system starts in idle state
        }

        [When(@"the start button is pressed")]
        public void WhenTheStartButtonIsPressed()
        {
            TrafficTestHooks.ClickButton("StartButton");
        }

        [Then(@"the red pedestrian light should be turned on")]
        public void ThenTheRedPedestrianLightShouldBeTurnedOn()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("PedestrianRed", "On");
        }

        [Given(@"the system is operational")]
        public void GivenTheSystemIsOperational()
        {
            TrafficTestHooks.ClickButton("StartButton");
            TrafficTestHooks.WaitUntilTheLightIsInThatState("PedestrianRed", "On");
        }

        [Given(@"the red pedestrian light is on")]
        public void GivenTheRedPedestrianLightIsOn()
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("PedestrianRed", "On");
        }

        [When(@"the pedestrian button is pressed")]
        public void WhenThePedestrianButtonIsPressed()
        {
            TrafficTestHooks.Client.PressRequestPedestrianWalkButton(new TestBus.Empty());
        }

        [Then(@"the green pedestrian light should be turned on within (\d+) seconds")]
        public void ThenTheGreenPedestrianLightShouldBeTurnedOnWithinSeconds(int seconds)
        {
            TrafficTestHooks.WaitUntilTheLightIsInThatState("PedestrianGreen", "On", seconds);
        }

        [Given(@"the green pedestrian light is on")]
        public void GivenTheGreenPedestrianLightIsOn()
        {
            GivenTheSystemIsOperational();
            WhenThePedestrianButtonIsPressed();
            ThenTheGreenPedestrianLightShouldBeTurnedOnWithinSeconds(10);
        }

        [When(@"the green pedestrian light on time has elapsed")]
        public void WhenTheGreenPedestrianLightOnTimeHasElapsed()
        {
            // Wait for the green light to start blinking
            TrafficTestHooks.WaitUntilTheLightIsInThatState("PedestrianGreen", "Off", 30);
            TrafficTestHooks.WaitUntilTheLightIsInThatState("PedestrianGreen", "On", 2);
        }

        [Then(@"the green pedestrian light should start blinking")]
        public void ThenTheGreenPedestrianLightShouldStartBlinking()
        {
            // Verify blinking by checking if the light alternates between on and off
            TrafficTestHooks.WaitUntilTheLightIsInThatState("PedestrianGreen", "Off", 2);
            TrafficTestHooks.WaitUntilTheLightIsInThatState("PedestrianGreen", "On", 2);
        }

        [When(@"the green pedestrian light on time and blinking time have elapsed")]
        public void WhenTheGreenPedestrianLightOnTimeAndBlinkingTimeHaveElapsed()
        {
            WhenTheGreenPedestrianLightOnTimeHasElapsed();
            // Wait for additional time to cover blinking period
            System.Threading.Thread.Sleep(10000);
        }

        [When(@"any pedestrian light is on")]
        public void WhenAnyPedestrianLightIsOn()
        {
            var lastState = TrafficTestHooks.TrafficLightStates.LastOrDefault();
            lastState.Should().NotBeNull();
            (lastState.PedestrianRed.LightState == "On" || lastState.PedestrianGreen.LightState == "On").Should().BeTrue();
        }

        [Then(@"only one pedestrian light should be on")]
        public void ThenOnlyOnePedestrianLightShouldBeOn()
        {
            var lastState = TrafficTestHooks.TrafficLightStates.LastOrDefault();
            lastState.Should().NotBeNull();
            (lastState.PedestrianRed.LightState == "On" ^ lastState.PedestrianGreen.LightState == "On").Should().BeTrue();
        }
    }
}
