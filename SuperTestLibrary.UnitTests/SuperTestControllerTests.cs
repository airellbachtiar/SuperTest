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
        private Mock<IReqIFStorage> _mockReqIFStorage;
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

        [Test]
        public async Task GenerateSpecFlowFeatureFileAsync_ValidInput_ReturnsFeatureFileResponse()
        {
            // Arrange
            _mockLLM.Setup(llm => llm.Id).Returns(LLMResponse.ValidId);
            _mockLLM.Setup(llm => llm.CallAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(LLMResponse.ValidLlmResponse);

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
    }
}
