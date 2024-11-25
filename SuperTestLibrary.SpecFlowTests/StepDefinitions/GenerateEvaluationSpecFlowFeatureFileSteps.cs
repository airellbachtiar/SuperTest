using Moq;
using LlmLibrary;
using SuperTestLibrary.Services.Prompts.ResponseModels;
using SuperTestLibrary.Storages;

namespace SuperTestLibrary.SpecFlowTests.StepDefinitions
{
    [Binding]
    [Scope(Feature = "Evaluate SpecFlow Feature File")]
    public class GenerateEvaluationSpecFlowFeatureFileSteps
    {
        private readonly SuperTestController _superTestController = new(new Mock<IReqIFStorage>().Object);
        private readonly Mock<ILargeLanguageModel> _mockLargeLanguageModel = new();
        private string _requirements = "The application should generate an evaluation score";
        private string _featureFile = "Feature: Generate Evaluation Score";
        private EvaluateSpecFlowFeatureFileResponse? _evaluateSpecFlowFeatureFileResponse = null;
        private string _errorMessage = string.Empty;

        private const string _llmId = "Claude 3.5 Sonnet";
        private const string _llmResponse = "{\"Readability\": 4, \"Consistency\": 4, \"Focus\": 4, \"Structure\": 0, \"Maintainability\": 0, \"Coverage\": 4, \"Score\": {\"MaximumScore\": 20, \"TotalScore\": 16}, \"Summary\": \"The feature file is generally well-written and meets most of the evaluation criteria. However, there are a few recommendations for improvement. The feature name is descriptive and provides a clear understanding of its purpose, resulting in high readability and focus. It maintains consistent naming and format, which adds to its clarity. There is no use of Background steps or step reusability in this feature file, which might reduce maintainability and the potential for streamlined steps across scenarios. Coverage is good, as it considers various scenarios including error conditions. Consider using Background steps if there are common preconditions across scenarios and explore step reuse opportunities to enhance maintainability.\"}";

        #region Generate evaluation score for a valid SpecFlow feature file

        [BeforeScenario("Generate evaluation score for a valid SpecFlow feature file")]
        public void SetupGenerateEvaluationScoreForAValidSpecFlowFeatureFile()
        {
            _mockLargeLanguageModel.Setup(llm => llm.Id).Returns(_llmId);
            _mockLargeLanguageModel.Setup(llm => llm.CallAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(_llmResponse);
            _superTestController.SelectedLLM = _mockLargeLanguageModel.Object;
        }

        [Given(@"I have a set of requirements")]
        public void GivenIHaveASetOfRequirements()
        {
            // No action needed
        }

        [Given(@"I have a valid SpecFlow feature file")]
        public void GivenIHaveAValidSpecFlowFeatureFile()
        {
            // No action needed
        }

        [When(@"I request an evaluation score")]
        public async Task WhenIRequestAnEvaluationScore()
        {
            try
            {
                _evaluateSpecFlowFeatureFileResponse = await _superTestController.EvaluateSpecFlowFeatureFileAsync(_requirements, _featureFile);
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
        }

        [Then(@"the application should generate an evaluation score")]
        public void ThenTheApplicationShouldGenerateAnEvaluationScore()
        {
            Assert.NotNull(_evaluateSpecFlowFeatureFileResponse);
        }
        #endregion

        #region Attempt to generate evaluation score without requirements
        [BeforeScenario("Attempt to generate evaluation score without requirements")]
        public void SetupAttemptToGenerateEvaluationScoreWithoutRequirements()
        {
            _superTestController.SelectedLLM = _mockLargeLanguageModel.Object;
        }

        [Given(@"I don't have any requirements")]
        public void GivenIDontHaveAnyRequirements()
        {
            _requirements = string.Empty;
        }

        [Then(@"the application should return an error")]
        public void ThenTheApplicationShouldReturnAnError()
        {
            Assert.NotEmpty(_errorMessage);
        }

        [Then(@"the error should indicate that requirements are missing")]
        public void ThenTheErrorShouldIndicateThatRequirementsAreMissing()
        {
            Assert.Contains("No requirements provided", _errorMessage);
        }
        #endregion

        #region Attempt to generate evaluation score without a SpecFlow feature file\
        [BeforeScenario("Attempt to generate evaluation score without a SpecFlow feature file")]
        public void SetupAttemptToGenerateEvaluationScoreWithoutASpecFlowFeatureFile()
        {
            _mockLargeLanguageModel.Setup(llm => llm.Id).Returns(_llmId);
            _superTestController.SelectedLLM = _mockLargeLanguageModel.Object;
        }

        [Given(@"I don't have a SpecFlow feature file")]
        public void GivenIDontHaveASpecFlowFeatureFile()
        {
            _featureFile = string.Empty;
        }

        [Then(@"the error should indicate that a SpecFlow feature file is missing")]
        public void ThenTheErrorShouldIndicateThatASpecFlowFeatureFileIsMissing()
        {
            Assert.Contains("No feature file provided", _errorMessage);
        }
        #endregion

        #region Generate evaluation score with detailed feedback
        [BeforeScenario("Generate evaluation score with detailed feedback")]
        public void SetupGenerateEvaluationScoreWithDetailedFeedback()
        {
            _mockLargeLanguageModel.Setup(llm => llm.Id).Returns(_llmId);
            _mockLargeLanguageModel.Setup(llm => llm.CallAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(_llmResponse);
            _superTestController.SelectedLLM = _mockLargeLanguageModel.Object;
        }

        [Then(@"provide detailed feedback")]
        public void ThenProvideDetailedFeedback()
        {
            Assert.NotEmpty(_evaluateSpecFlowFeatureFileResponse!.Summary);
        }
        #endregion

    }
}
