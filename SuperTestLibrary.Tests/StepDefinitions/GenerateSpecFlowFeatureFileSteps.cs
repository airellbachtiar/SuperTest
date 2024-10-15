using Moq;
using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services;
using SuperTestLibrary.Services.Prompts;
using SuperTestLibrary.Storages;

namespace SuperTestLibrary.Tests.StepDefinitions
{
    [Binding]
    public class GenerateSpecFlowFeatureFileStepDefinitions
    {
        private readonly SuperTestController _superTestController = new SuperTestController(new Mock<IReqIFStorage>().Object);
        private SpecFlowFeatureFileGenerator _featureFileGenerator = new SpecFlowFeatureFileGenerator();
        private readonly Mock<ILargeLanguageModel> _mockLargeLanguageModel = new Mock<ILargeLanguageModel>();
        private string? _requirements = string.Empty;
        private SpecFlowFeatureFileResponse? _generatedFeatureFile;
        private string _errorMessage = string.Empty;

        [Given(@"I am using the application")]
        public void GivenIAmUsingTheApplication()
        {
            _superTestController.SetLLM(_mockLargeLanguageModel.Object);
            _superTestController.SetGenerator(_featureFileGenerator);
        }

        [Given(@"I have a set of requirements")]
        public void GivenIHaveASetOfRequirements()
        {
            _requirements = "The application should generate SpecFlow feature file";
            MockLargeLanguageModel();
        }

        [Given(@"I have chosen an LLM for generation")]
        public void GivenIHaveChosenAnLLMForGeneration()
        {
            _superTestController.SetLLM(_mockLargeLanguageModel.Object);
        }

        [When(@"I request to generate a SpecFlow feature file")]
        public async Task WhenIRequestToGenerateASpecFlowFeatureFile()
        {
            try
            {
                _superTestController.SetGenerator(_featureFileGenerator);
                _generatedFeatureFile = await _superTestController.GenerateSpecFlowFeatureFileAsync(_requirements);
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
        }

        [Then(@"the application should generate a valid SpecFlow feature file")]
        public void ThenTheApplicationShouldGenerateAValidSpecFlowFeatureFile()
        {
            Assert.NotNull(_generatedFeatureFile);
            // TODO: Implement validation logic
            //Assert.IsTrue(_superTestController.ValidateFeatureFile(_generatedFeatureFile));
        }

        [Then(@"the generated file should reflect the provided requirements")]
        public void ThenTheGeneratedFileShouldReflectTheProvidedRequirements()
        {
            Assert.True(_generatedFeatureFile!.FeatureFiles.Values.First().Contains(_requirements), "generate SpecFlow feature file");
        }

        [When(@"I select an LLM from the available options")]
        public void WhenISelectAnLLMFromTheAvailableOptions()
        {
            _superTestController.SetLLM(_mockLargeLanguageModel.Object);
        }

        [Then(@"the application should use the selected LLM for feature file generation")]
        public void ThenTheApplicationShouldUseTheSelectedLLMForFeatureFileGeneration()
        {
            // TODO: Implement this
            //Assert.NotNull(_superTestController.SelectedLLM);
        }

        [When(@"I select a SpecFlow feature file generator")]
        public void WhenISelectASpecFlowFeatureFileGenerator()
        {
            _superTestController.SetGenerator(_featureFileGenerator);
        }

        [Then(@"the application should use the selected generator for creating feature files")]
        public void ThenTheApplicationShouldUseTheSelectedGeneratorForCreatingFeatureFiles()
        {
            // TODO: Implement this
            //Assert.NotNull(_superTestController.SelectedGenerator);
        }

        [Given(@"I have an invalid set of requirements")]
        public void GivenIHaveAnInvalidSetOfRequirements()
        {
            _requirements = null;  // Simulate invalid input
            MockLargeLanguageModel();
        }

        [Then(@"the application should handle the error gracefully")]
        public void ThenTheApplicationShouldHandleTheErrorGracefully()
        {
            Assert.NotEmpty(_errorMessage);
        }

        [Then(@"provide an appropriate error message")]
        public void ThenProvideAnAppropriateErrorMessage()
        {
            Assert.Contains("requirements are invalid.", _errorMessage);
        }

        [Given(@"I have an empty set of requirements")]
        public void GivenIHaveAnEmptySetOfRequirements()
        {
            _requirements = string.Empty;
            MockLargeLanguageModel();
            _superTestController.SetLLM(_mockLargeLanguageModel.Object);
        }

        [Then(@"the application should handle the situation appropriately")]
        public void ThenTheApplicationShouldHandleTheSituationAppropriately()
        {
            Assert.NotEmpty(_errorMessage);
        }

        [Then(@"provide feedback that no feature file can be generated")]
        public void ThenProvideFeedbackThatNoFeatureFileCanBeGenerated()
        {
            Assert.Contains("requirements are empty.", _errorMessage);
        }

        private void MockLargeLanguageModel()
        {
            _mockLargeLanguageModel.Setup(llm => llm.Id).Returns("GPT-4o");
            _mockLargeLanguageModel.Setup(llm => llm.Call(It.IsAny<IEnumerable<string>>())).ReturnsAsync("Feature: The application should generate SpecFlow feature file");
        }
    }
}
