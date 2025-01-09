using Microsoft.Extensions.Logging;
using Moq;
using SuperTestLibrary;
using SuperTestLibrary.Models;
using SuperTestWPF.Models;
using SuperTestWPF.Retry;
using SuperTestWPF.Services;
using SuperTestWPF.UnitTests.Helper;
using System.Collections.ObjectModel;

namespace SuperTestWPF.UnitTests.ServiceTests
{
    public class EvaluateFeatureFileServiceTests
    {
        private Mock<ISuperTestController> _mockController;
        private Mock<ILogger<EvaluateFeatureFileService>> _mockLogger;
        private Mock<IRetryService> _mockRetry;
        private EvaluateFeatureFileService _service;

        [SetUp]
        public void SetUp()
        {
            _mockController = new Mock<ISuperTestController>();
            _mockLogger = new Mock<ILogger<EvaluateFeatureFileService>>();
            _mockRetry = new Mock<IRetryService>();

            _service = new EvaluateFeatureFileService(_mockController.Object, _mockLogger.Object, _mockRetry.Object);
        }

        [Test]
        public async Task EvaluateFeatureFileAsync_ValidInputs_ShouldReturnPromptHistories()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            const string requirements = "Requirements content";

            var featureFile = new SpecFlowFeatureFileModel
            {
                FeatureFileName = "TestFeature.feature",
                FeatureFileContent = "Feature file content",
                Scenarios = []
            };

            var mockResponse = new EvaluateSpecFlowFeatureFileResponse
            {
                Prompts = ["Prompt1", "Prompt2"]
            };

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<EvaluateSpecFlowFeatureFileResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _service.EvaluateFeatureFileAsync(selectedLlmString, featureFile, requirements);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(mockResponse.Prompts.Count));
            Assert.That(result.First().Prompt, Is.EqualTo("Prompt1"));
        }

        [Test]
        public void EvaluateFeatureFileAsync_Cancellation_ShouldLogWarning()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            const string requirements = "Requirements content";

            var featureFile = new SpecFlowFeatureFileModel
            {
                FeatureFileName = "TestFeature.feature",
                FeatureFileContent = "Feature file content"
            };

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<EvaluateSpecFlowFeatureFileResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await _service.EvaluateFeatureFileAsync(selectedLlmString, featureFile, requirements, cancellationTokenSource.Token));
            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Warning, "Operation was cancelled."), Is.True);
        }

        [Test]
        public void EvaluateFeatureFileAsync_Exception_ShouldLogError()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            const string requirements = "Requirements content";

            var featureFile = new SpecFlowFeatureFileModel
            {
                FeatureFileName = "TestFeature.feature",
                FeatureFileContent = "Feature file content"
            };

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<EvaluateSpecFlowFeatureFileResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));

            _mockController
                .Setup(c => c.SelectedLLM!.Id)
                .Returns(selectedLlmString);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await _service.EvaluateFeatureFileAsync(selectedLlmString, featureFile, requirements));
            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Error, $"Exception while evaluating {featureFile.FeatureFileName} using {selectedLlmString}"), Is.True);
        }

        [Test]
        public async Task EvaluateSpecFlowScenarioAsync_ValidInputs_ShouldReturnPromptHistories()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            const string requirements = "Requirements content";

            var featureFile = new SpecFlowFeatureFileModel
            {
                FeatureFileName = "TestFeature.feature",
                FeatureFileContent = "Feature file content",
                Scenarios = new ObservableCollection<ScenarioModel>
                {
                    new ScenarioModel { Name = "Scenario1" },
                    new ScenarioModel { Name = "Scenario2" }
                }
            };

            var mockResponse = new EvaluateSpecFlowScenarioResponse
            {
                Prompts = new List<string> { "Prompt1", "Prompt2" },
                ScenarioEvaluations = new List<SpecFlowScenarioEvaluation>
                {
                    new SpecFlowScenarioEvaluation { ScenarioName = "Scenario1" },
                    new SpecFlowScenarioEvaluation { ScenarioName = "Scenario2" }
                }
            };

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<EvaluateSpecFlowScenarioResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _service.EvaluateSpecFlowScenarioAsync(selectedLlmString, featureFile, requirements);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(mockResponse.Prompts.Count));
            Assert.That(result.First().Prompt, Is.EqualTo("Prompt1"));
        }

        [Test]
        public void EvaluateSpecFlowScenarioAsync_Cancellation_ShouldLogWarning()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            const string requirements = "Requirements content";

            var featureFile = new SpecFlowFeatureFileModel
            {
                FeatureFileName = "TestFeature.feature",
                FeatureFileContent = "Feature file content"
            };

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<EvaluateSpecFlowScenarioResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await _service.EvaluateSpecFlowScenarioAsync(selectedLlmString, featureFile, requirements, cancellationTokenSource.Token));
            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Warning, "Operation was cancelled."), Is.True);
        }

        [Test]
        public void EvaluateSpecFlowScenarioAsync_Exception_ShouldLogError()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            const string requirements = "Requirements content";

            var featureFile = new SpecFlowFeatureFileModel
            {
                FeatureFileName = "TestFeature.feature",
                FeatureFileContent = "Feature file content"
            };

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<EvaluateSpecFlowScenarioResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));

            _mockController
                .Setup(c => c.SelectedLLM!.Id)
                .Returns(selectedLlmString);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await _service.EvaluateSpecFlowScenarioAsync(selectedLlmString, featureFile, requirements));

            _mockLogger.Verify(log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Exception while evaluating {featureFile.FeatureFileName}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Error, $"Exception while evaluating {featureFile.FeatureFileName} using {selectedLlmString}"), Is.True);
        }
    }
}
