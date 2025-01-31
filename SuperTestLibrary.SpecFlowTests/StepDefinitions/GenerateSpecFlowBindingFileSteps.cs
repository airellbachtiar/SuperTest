using FluentAssertions;
using SuperTestLibrary.Models;
using LargeLanguageModelLibrary;
using Moq;
using Microsoft.Extensions.Logging;
using SuperTestLibrary.Storages;
using LargeLanguageModelLibrary.Enums;
using LargeLanguageModelLibrary.Models;

namespace SuperTestLibrary.SpecFlowTests.StepDefinitions
{
    [Binding]
    [Scope(Feature = "Generate SpecFlow Binding File")]
    public class GenerateSpecFlowBindingFileSteps
    {
        private SuperTestController _superTestController;
        private readonly Mock<IReqIFStorage> _mockReqIFStorage = new();
        private readonly Mock<ILogger<SuperTestController>> _mockLogger = new();
        private readonly Mock<ILargeLanguageModel> _mockLargeLanguageModel = new();
        private SpecFlowFeatureFileResponse? _featureFile;
        private Dictionary<string, string> _testInterfaces = [];
        private SpecFlowBindingFileResponse? _bindingFileResponse;

        private const string _llmId = "Claude 3.5 Sonnet";
        private static MessageResponse _llmResponse = new()
        {
            Messages =
            [
                new()
                {
                    Text = "{\r\n  \"BindingFiles\": {\r\n    \"PedestrianLightSystemSteps.cs\": \"using FluentAssertions;\\nusing NUnit.Framework;\\nusing TechTalk.SpecFlow;\\nusing TrafficTest;\\n\\nnamespace TrafficTest.Steps\\n{\\n    [Binding]\\n    [Scope(Feature = \\\"Pedestrian Light System\\\")]\\n    public class PedestrianLightSystemSteps\\n    {\\n        [Given(@\\\"the system is in idle state\\\")]\\n        public void GivenTheSystemIsInIdleState()\\n        {\\n            // Assuming the system starts in idle state\\n            // No action needed\\n        }\\n\\n        [When(@\\\"the start button is pressed\\\")]\\n        public void WhenTheStartButtonIsPressed()\\n        {\\n            TrafficTestHooks.ClickButton(\\\"StartButton\\\");\\n        }\\n\\n        [Then(@\\\"the red pedestrian light should be turned on\\\")]\\n        public void ThenTheRedPedestrianLightShouldBeTurnedOn()\\n        {\\n            TrafficTestHooks.WaitUntilTheLightIsInThatState(\\\"PedestrianRed\\\", \\\"On\\\");\\n        }\\n\\n        [Given(@\\\"the system is operational\\\")]\\n        public void GivenTheSystemIsOperational()\\n        {\\n            // Assuming the system is operational after pressing the start button\\n            TrafficTestHooks.ClickButton(\\\"StartButton\\\");\\n        }\\n\\n        [Given(@\\\"the red pedestrian light is on\\\")]\\n        public void GivenTheRedPedestrianLightIsOn()\\n        {\\n            TrafficTestHooks.WaitUntilTheLightIsInThatState(\\\"PedestrianRed\\\", \\\"On\\\");\\n        }\\n\\n        [When(@\\\"the pedestrian button is pressed\\\")]\\n        public void WhenThePedestrianButtonIsPressed()\\n        {\\n            TrafficTestHooks.Client.PressRequestPedestrianWalkButton(new TestBus.Empty());\\n        }\\n\\n        [Then(@\\\"the green pedestrian light should be turned on within (\\\\d+) seconds\\\")]\\n        public void ThenTheGreenPedestrianLightShouldBeTurnedOnWithinSeconds(int seconds)\\n        {\\n            TrafficTestHooks.WaitUntilTheLightIsInThatState(\\\"PedestrianGreen\\\", \\\"On\\\", seconds);\\n        }\\n\\n        [Given(@\\\"the green pedestrian light is on\\\")]\\n        public void GivenTheGreenPedestrianLightIsOn()\\n        {\\n            TrafficTestHooks.WaitUntilTheLightIsInThatState(\\\"PedestrianGreen\\\", \\\"On\\\");\\n        }\\n\\n        [When(@\\\"the green pedestrian light on time has elapsed\\\")]\\n        public void WhenTheGreenPedestrianLightOnTimeHasElapsed()\\n        {\\n            // Assuming the green light on time is handled by the system\\n            // We'll wait for the green light to start blinking\\n        }\\n\\n        [Then(@\\\"the green pedestrian light should start blinking\\\")]\\n        public void ThenTheGreenPedestrianLightShouldStartBlinking()\\n        {\\n            // Assuming blinking is represented by rapid on/off changes\\n            // We'll check for alternating states within a short timeframe\\n            var initialState = TrafficTestHooks.TrafficLightStates.Last().PedestrianGreen.LightState;\\n            TrafficTestHooks.WaitUntilTheLightIsInThatState(\\\"PedestrianGreen\\\", initialState == \\\"On\\\" ? \\\"Off\\\" : \\\"On\\\", 5);\\n        }\\n\\n        [When(@\\\"the green pedestrian light on time and blinking time have elapsed\\\")]\\n        public void WhenTheGreenPedestrianLightOnTimeAndBlinkingTimeHaveElapsed()\\n        {\\n            // Assuming the system handles the timing\\n            // We'll wait for the red light to turn on\\n        }\\n\\n        [When(@\\\"any pedestrian light is on\\\")]\\n        public void WhenAnyPedestrianLightIsOn()\\n        {\\n            var lastState = TrafficTestHooks.TrafficLightStates.Last();\\n            (lastState.PedestrianRed.LightState == \\\"On\\\" || lastState.PedestrianGreen.LightState == \\\"On\\\")\\n                .Should().BeTrue(\\\"At least one pedestrian light should be on\\\");\\n        }\\n\\n        [Then(@\\\"only one pedestrian light should be on\\\")]\\n        public void ThenOnlyOnePedestrianLightShouldBeOn()\\n        {\\n            var lastState = TrafficTestHooks.TrafficLightStates.Last();\\n            (lastState.PedestrianRed.LightState == \\\"On\\\" ^ lastState.PedestrianGreen.LightState == \\\"On\\\")\\n                .Should().BeTrue(\\\"Exactly one pedestrian light should be on\\\");\\n        }\\n    }\\n}\\n\"\r\n  }\r\n}"
                }
            ]
        };

        public GenerateSpecFlowBindingFileSteps()
        {
            _superTestController = new SuperTestController(_mockReqIFStorage.Object, _mockLargeLanguageModel.Object, _mockLogger.Object);
        }

        [BeforeScenario()]
        public void Setup()
        {
            _mockLargeLanguageModel.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(_llmResponse);
            _superTestController.SelectedLLM = ModelName.GPT4o;
        }

        #region Generate SpecFlow binding file from feature file and test interfaces
        [Given(@"I have a SpecFlow feature file")]
        public void GivenIHaveASpecFlowFeatureFile()
        {
            _featureFile = new SpecFlowFeatureFileResponse
            {
                FeatureFiles = new Dictionary<string, string>
                {
                    { "GenerateSpecFlowBindingFile.feature", "Feature: Generate SpecFlow Binding File" }
                }
            };
        }

        [Given(@"I have test interfaces")]
        public void GivenIHaveTestInterfaces()
        {
            _testInterfaces.Add("TestInterface.cs", "public interface ITestInterface { }");
        }

        [When(@"I request to generate a SpecFlow binding file")]
        public async Task WhenIRequestToGenerateASpecFlowBindingFile()
        {
            _superTestController.SelectedLLM = ModelName.GPT4o;

            _bindingFileResponse = await _superTestController.GenerateSpecFlowBindingFileAsync(_featureFile!.FeatureFiles.First().Value, _testInterfaces);
        }

        [Then(@"a SpecFlow binding file should be generated")]
        public void ThenASpecFlowBindingFileShouldBeGenerated()
        {
            _bindingFileResponse.Should().NotBeNull();
            _bindingFileResponse!.BindingFiles.Should().NotBeEmpty();
        }
        #endregion

        #region Choose LLM for generating SpecFlow binding file
        [When(@"I select an LLM for generating the SpecFlow binding file")]
        public void WhenISelectAnLLMForGeneratingTheSpecFlowBindingFile()
        {
            _superTestController.SelectedLLM = ModelName.GPT4o;
        }

        [Then(@"the selected LLM should be used to generate the binding file")]
        public void ThenTheSelectedLLMShouldBeUsedToGenerateTheBindingFile()
        {
            Assert.NotNull(_superTestController.SelectedLLM);
        }
        #endregion
    }
}
