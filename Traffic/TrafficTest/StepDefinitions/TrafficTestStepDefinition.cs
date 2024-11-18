using FlaUI.Core.AutomationElements;
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
            
        }

        [Then("the traffic light should be red")]
        public void ThenTheTrafficLightShouldBeRed()
        {
            string automationId = "StartButton";
            var mainWindow = SpecFlowHooks.App.GetMainWindow(SpecFlowHooks.Automation)
                            ?? throw new Exception("Main window could not be found.");

            // Find the button using its AutomationId
            var button = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsButton()
                         ?? throw new Exception($"Button with AutomationId '{automationId}' could not be found.");

            // Simulate button click
            button.Click();

            Thread.Sleep(4000);

            var stopButton = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("StopButton"))?.AsButton()
                             ?? throw new Exception($"Button with AutomationId 'StopButton' could not be found.");

            stopButton.Click();

            string trafficLightState = _testAccess.GetCarRedCloseState();
            Assert.AreEqual("Open", trafficLightState);
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
            string automationId = "StartButton";
            var mainWindow = SpecFlowHooks.App.GetMainWindow(SpecFlowHooks.Automation)
                            ?? throw new Exception("Main window could not be found.");

            // Find the button using its AutomationId
            var button = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsButton()
                         ?? throw new Exception($"Button with AutomationId '{automationId}' could not be found.");

            // Simulate button click
            button.Click();

            Thread.Sleep(1500);

            var stopButton = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("StopButton"))?.AsButton()
                             ?? throw new Exception($"Button with AutomationId 'StopButton' could not be found.");

            stopButton.Click();

            string pedLightState = _testAccess.GetPedestrianRedCloseState();
            Assert.AreEqual("Open", pedLightState);
        }
        #endregion
    }
}
