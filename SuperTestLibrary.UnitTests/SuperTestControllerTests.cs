using Gherkin;
using Moq;
using SuperTestLibrary.Helpers;
using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services.Prompts.ResponseModels;
using SuperTestLibrary.Storages;
using SuperTestLibrary.UnitTests.TestData;
using System.Text.Json;

namespace SuperTestLibrary.UnitTests
{
    public sealed class SuperTestControllerTests
    {
        private Mock<IReqIFStorage> _mockReqIFStorage = new();
        private Mock<ILargeLanguageModel> _mockLLM = new();
        private SuperTestController _controller;

        [SetUp]
        public void Setup()
        {
            _mockReqIFStorage = new Mock<IReqIFStorage>();
            _controller = new SuperTestController(_mockReqIFStorage.Object);
            _mockLLM = new Mock<ILargeLanguageModel>();
            _controller.SelectedLLM = _mockLLM.Object;
        }

        #region GenerateSpecFlowFeatureFileAsync Tests
        [Test]
        public async Task GenerateSpecFlowFeatureFileAsync_ValidInput_ReturnsFeatureFileResponse()
        {
            // Arrange
            _mockLLM.Setup(llm => llm.Id).Returns(LLMResponse.ValidId);
            _mockLLM.Setup(llm => llm.CallAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(LLMResponse.ValidSpecFlowFeatureFileResponse);

            // Act
            SpecFlowFeatureFileResponse result = await _controller.GenerateSpecFlowFeatureFileAsync(Requirement.ValidRequirement);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.GherkinDocuments, Is.Not.Empty);
        }

        [Test]
        public void GenerateSpecFlowFeatureFileAsync_InvalidInput_ThrowsInvalidOperationException()
        {
            // Arrange
            string invalidRequirements = string.Empty;

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.GenerateSpecFlowFeatureFileAsync(invalidRequirements));
        }

        [Test]
        public void GenerateSpecFlowFeatureFileAsync_IncompleteLlmResponse_ThrowsJsonException()
        {
            //Arrange
            _mockLLM.Setup(llm => llm.Id).Returns(LLMResponse.ValidId);
            _mockLLM.Setup(llm => llm.CallAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(LLMResponse.IncompleteFeatureFile);

            // Act & Assert
            Assert.ThrowsAsync<JsonException>(async () =>
                await _controller.GenerateSpecFlowFeatureFileAsync(Requirement.ValidRequirement));
        }

        [Test]
        public void GenerateSpecFlowFeatureFileAsync_InvalidSpecFlowFileResponse_ThrowsCompositeParserException()
        {
            // Arrange
            _mockLLM.Setup(llm => llm.Id).Returns(LLMResponse.ValidId);
            _mockLLM.Setup(llm => llm.CallAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(LLMResponse.InvalidSpecFlowFeatureFileResponse);

            // Act & Assert
            Assert.ThrowsAsync<CompositeParserException>(async () =>
                await _controller.GenerateSpecFlowFeatureFileAsync(Requirement.ValidRequirement));
        }
        #endregion

        #region EvaluateSpecFlowFeatureFileAsync Tests
        [Test]
        public async Task EvaluateSpecFlowFeatureFileAsync_ValidInput_ReturnsSpecFlowFeatureFileResponse()
        {
            // Arrange
            var expectedResult = new EvaluateSpecFlowFeatureFileResponse
            {
                Score = new EvaluationScore
                {
                    //Score = 0.5,
                    //ScoreType = "FeatureFile"
                }
            };
            _mockLLM.Setup(llm => llm.Id).Returns(LLMResponse.ValidId);
            _mockLLM.Setup(llm => llm.CallAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(LLMResponse.ValidEvaluateSpecFlowFeatureFileResponse);

            // Act
            EvaluateSpecFlowFeatureFileResponse result = await _controller.EvaluateSpecFlowFeatureFileAsync(Requirement.ValidRequirement, FeatureFile.ValidSpecFlowFeatureFile);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Score, Is.Not.Null);
        }

        [Test]
        public void EvaluateSpecFlowFeatureFileAsync_InvalidInput_ThrowsInvalidOperationException()
        {
            // Arrange
            string invalidRequirements = string.Empty;

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.EvaluateSpecFlowFeatureFileAsync(invalidRequirements, FeatureFile.ValidSpecFlowFeatureFile));
        }

        [Test]
        public void EvaluateSpecFlowFeatureFileAsync_InvalidFeatureFile_ThrowsInvalidOperationException()
        {
            // Arrange
            string invalidFeatureFile = string.Empty;
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.EvaluateSpecFlowFeatureFileAsync(Requirement.ValidRequirement, invalidFeatureFile));
        }

        [Test]
        public void EvaluateSpecFlowFeatureFileAsync_IncompleteLlmResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockLLM.Setup(llm => llm.Id).Returns(LLMResponse.ValidId);
            _mockLLM.Setup(llm => llm.CallAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(LLMResponse.IncompleteEvaluateSpecFlowFeatureFileResponse);
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.EvaluateSpecFlowFeatureFileAsync(Requirement.ValidRequirement, FeatureFile.ValidSpecFlowFeatureFile));
        }

        [Test]
        public void EvaluateSpecFlowFeatureFileAsync_InvalidEvaluateSpecFlowFeatureFileResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockLLM.Setup(llm => llm.Id).Returns(LLMResponse.ValidId);
            _mockLLM.Setup(llm => llm.CallAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(LLMResponse.MismatchEvaluateSpecFlowFeatureFileResponse);
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.EvaluateSpecFlowFeatureFileAsync(Requirement.ValidRequirement, FeatureFile.ValidSpecFlowFeatureFile));
        }
        #endregion
    }
}
