using FlaUI.Core.AutomationElements;
using NUnit.Framework;
using TestBus;

namespace TrafficTest.StepDefinitions
{
    [Binding]
    public class TrafficTestStepDefinition
    {

        #region Traffic light system starts in red light
        [Given("the traffic light system starts")]
        public void GivenTheTrafficLightSystemStarts()
        {
            ClickButton("StartButton");
            Thread.Sleep(3000);
        }

        [When("the system initializes")]
        public void WhenTheSystemInitializes()
        {
            
        }

        [Then("the traffic light should be red")]
        public void ThenTheTrafficLightShouldBeRed()
        {
            var response = SpecFlowHooks.Client.Test(new Empty());
            var carRedLightResponse = SpecFlowHooks.Client.GetCarRedLightState(new Empty());
            var carYellowLightResponse = SpecFlowHooks.Client.GetCarYellowLightState(new Empty());
            var carGreenLightResponse = SpecFlowHooks.Client.GetCarGreenLightState(new Empty());
            var pedRedLightResponse = SpecFlowHooks.Client.GetPedestrianRedLightState(new Empty());

            Assert.AreEqual(42, response.ResponseCode);
            Assert.AreEqual("On", carRedLightResponse.LightState);
            Assert.AreEqual("Off", carYellowLightResponse.LightState);
            Assert.AreEqual("Off", carGreenLightResponse.LightState);
            Assert.AreEqual("Off", pedRedLightResponse.LightState);
        }
        #endregion

        #region Pedestrian light system starts in red light
        [Given("the pedestrian light system starts")]
        public void GivenThePedestrianLightSystemStarts()
        {
            ClickButton("StartButton");
        }

        [Then("the pedestrian light should be red")]
        public void ThenThePedestrianLightShouldBeRed()
        {
            var response = SpecFlowHooks.Client.Test(new Empty());
            var carRedLightResponse = SpecFlowHooks.Client.GetCarRedLightState(new Empty());
            var carYellowLightResponse = SpecFlowHooks.Client.GetCarYellowLightState(new Empty());
            var carGreenLightResponse = SpecFlowHooks.Client.GetCarGreenLightState(new Empty());
            var pedRedLightResponse = SpecFlowHooks.Client.GetPedestrianRedLightState(new Empty());

            Assert.AreEqual(42, response.ResponseCode);
            Assert.AreEqual("Off", carRedLightResponse.LightState);
            Assert.AreEqual("Off", carYellowLightResponse.LightState);
            Assert.AreEqual("On", carGreenLightResponse.LightState);
            Assert.AreEqual("On", pedRedLightResponse.LightState);
        }
        #endregion

        private void ClickButton(string automationId)
        {
            var mainWindow = SpecFlowHooks.App.GetMainWindow(SpecFlowHooks.Automation)
                            ?? throw new Exception("Main window could not be found.");

            // Find the button using its AutomationId
            var button = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsButton()
                         ?? throw new Exception($"Button with AutomationId '{automationId}' could not be found.");

            // Simulate button click
            button.Click();
        }
    }
}
