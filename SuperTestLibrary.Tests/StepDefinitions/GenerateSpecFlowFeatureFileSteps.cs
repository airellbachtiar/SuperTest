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
            // No action needed
        }

        [Given(@"I have a set of requirements")]
        public void GivenIHaveASetOfRequirements()
        {
            _requirements = "The application should generate SpecFlow feature file";
            MockLargeLanguageModel();
            ControllerSetup();
        }

        [Given(@"I have chosen an LLM for generation")]
        public void GivenIHaveChosenAnLLMForGeneration()
        {
            _superTestController.SelectedLLM = _mockLargeLanguageModel.Object;
        }

        [When(@"I request to generate a SpecFlow feature file")]
        public async Task WhenIRequestToGenerateASpecFlowFeatureFile()
        {
            try
            {
                _generatedFeatureFile = await _superTestController.GenerateSpecFlowFeatureFileAsync(_requirements!);
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
            Assert.True(_featureFileGenerator.ValidateFeatureFile(_generatedFeatureFile));
        }

        [Then(@"the generated file should reflect the provided requirements")]
        public void ThenTheGeneratedFileShouldReflectTheProvidedRequirements()
        {
            Assert.Contains("generate specflow feature file", _generatedFeatureFile!.FeatureFiles.Values.First().ToLower());
        }

        [When(@"I select an LLM from the available options")]
        public void WhenISelectAnLLMFromTheAvailableOptions()
        {
            _superTestController.SelectedLLM = _mockLargeLanguageModel.Object;
        }

        [Then(@"the application should use the selected LLM for feature file generation")]
        public void ThenTheApplicationShouldUseTheSelectedLLMForFeatureFileGeneration()
        {
            Assert.NotNull(_superTestController.SelectedLLM);
        }

        [When(@"I select a SpecFlow feature file generator")]
        public void WhenISelectASpecFlowFeatureFileGenerator()
        {
            _superTestController.SelectedGenerator = _featureFileGenerator;
        }

        [Then(@"the application should use the selected generator for creating feature files")]
        public void ThenTheApplicationShouldUseTheSelectedGeneratorForCreatingFeatureFiles()
        {
            Assert.NotNull(_superTestController.SelectedGenerator);
        }

        [Given(@"I have an invalid set of requirements")]
        public void GivenIHaveAnInvalidSetOfRequirements()
        {
            _requirements = null; // Invalid requirements
            MockLargeLanguageModel();
            ControllerSetup();
        }

        [Then(@"the application should handle the error gracefully")]
        public void ThenTheApplicationShouldHandleTheErrorGracefully()
        {
            Assert.NotEmpty(_errorMessage);
        }

        [Then(@"provide an appropriate error message")]
        public void ThenProvideAnAppropriateErrorMessage()
        {
            Assert.Contains("No requirements provided.", _errorMessage);
        }

        [Given(@"I have an empty set of requirements")]
        public void GivenIHaveAnEmptySetOfRequirements()
        {
            _requirements = string.Empty;
            MockLargeLanguageModel();
            ControllerSetup();
        }

        [Then(@"the application should handle the situation appropriately")]
        public void ThenTheApplicationShouldHandleTheSituationAppropriately()
        {
            Assert.NotEmpty(_errorMessage);
        }

        [Then(@"provide feedback that no feature file can be generated")]
        public void ThenProvideFeedbackThatNoFeatureFileCanBeGenerated()
        {
            Assert.Contains("No requirements provided.", _errorMessage);
        }

        private void MockLargeLanguageModel()
        {
            _mockLargeLanguageModel.Setup(llm => llm.Id).Returns("GPT-4o");
            _mockLargeLanguageModel.Setup(llm => llm.Call(It.IsAny<IEnumerable<string>>())).ReturnsAsync("{\r\n  \"FeatureFiles\": {\r\n    \"GenerateSpecFlowFeatureFile.feature\": \"Feature: Generate SpecFlow Feature File Using LLM\\n\\n  As a user\\n  I want to generate SpecFlow feature files using an LLM\\n  So that I can automate the creation of test scenarios based on requirements\\n\\n  Scenario: Generate SpecFlow feature file from requirements\\n    Given I have a set of requirements\\n    When I use the chosen LLM to generate a SpecFlow feature file\\n    Then a valid SpecFlow feature file should be created\\n    And the feature file should accurately reflect the given requirements\\n\\n  Scenario: Choose an LLM for feature file generation\\n    Given I have access to multiple LLMs\\n    When I select a specific LLM for feature file generation\\n    Then the selected LLM should be used to generate the SpecFlow feature file\\n\\n  Scenario: Choose SpecFlow feature file generator\\n    Given I have multiple SpecFlow feature file generators available\\n    When I select a specific SpecFlow feature file generator\\n    Then the selected generator should be used to create the feature file\\n\\n  Scenario Outline: Generate feature file with different LLMs\\n    Given I have requirements for a feature\\n    And I have selected <LLM> as the language model\\n    When I generate a SpecFlow feature file\\n    Then the feature file should be created using <LLM>\\n\\n    Examples:\\n      | LLM       |\\n      | GPT-3     |\\n      | GPT-4     |\\n      | BERT      |\\n      | T5        |\\n\\n  Scenario: Validate generated feature file\\n    Given a SpecFlow feature file has been generated\\n    When I review the generated feature file\\n    Then it should contain valid Gherkin syntax\\n    And it should include scenarios covering the provided requirements\\n\\n  # The following scenarios are recommendations and not directly derived from the given requirements\\n  # Scenario: Handle invalid requirements input\\n  #   Given I have an invalid set of requirements\\n  #   When I attempt to generate a SpecFlow feature file\\n  #   Then the system should provide an error message\\n  #   And no feature file should be generated\\n\\n  # Scenario: Generate feature file with empty requirements\\n  #   Given I have no requirements provided\\n  #   When I attempt to generate a SpecFlow feature file\\n  #   Then the system should prompt for requirements input\\n  #   And no feature file should be generated until requirements are provided\\n\\n  # Scenario: Compare output from different LLMs\\n  #   Given I have a set of requirements\\n  #   When I generate SpecFlow feature files using different LLMs\\n  #   Then I should be able to compare the outputs\\n  #   And select the most suitable generated feature file\"\r\n  }\r\n}");
        }

        private void ControllerSetup()
        {
            _superTestController.SelectedLLM = _mockLargeLanguageModel.Object;
            _superTestController.SelectedGenerator = _featureFileGenerator;
        }
    }
}
