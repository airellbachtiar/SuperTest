using Microsoft.Extensions.Logging;
using Moq;
using SuperTestLibrary;
using SuperTestLibrary.Helpers;
using SuperTestWPF.Models;
using SuperTestWPF.Retry;
using SuperTestWPF.Services;
using SuperTestWPF.UnitTests.Helper;

namespace SuperTestWPF.UnitTests.ServiceTests
{
    public class FeatureFileGeneratorServiceTests
    {
        private Mock<ISuperTestController> _mockController;
        private Mock<ILogger<FeatureFileGeneratorService>> _mockLogger;
        private Mock<IRetryService> _mockRetry;
        private FeatureFileGeneratorService _service;
        string validSpecFlowFeatureFile = "Feature: Enhanced Traffic Light and Pedestrian Light System\r\n\r\n  @REQ-001\r\n  Scenario: Traffic light system starts in red light\r\n    Given the traffic light system starts\r\n    When the system initializes\r\n    Then the traffic light should be red\r\n\r\n  @REQ-008\r\n  Scenario: Pedestrian light system starts in red light\r\n    Given the pedestrian light system starts\r\n    When the system initializes\r\n    Then the pedestrian light should be red";

        [SetUp]
        public void SetUp()
        {
            _mockController = new Mock<ISuperTestController>();
            _mockLogger = new Mock<ILogger<FeatureFileGeneratorService>>();
            _mockRetry = new Mock<IRetryService>();

            _service = new FeatureFileGeneratorService(_mockController.Object, _mockLogger.Object, _mockRetry.Object);
        }

        [Test]
        public async Task GenerateSpecFlowFeatureFilesAsync_ValidResponse_ShouldReturnFeatureFiles()
        {
            // Arrange
            const string requirements = "Test requirements";
            const string selectedLlmString = "GPT-4o";
            var cancellationToken = CancellationToken.None;
            var featureFiles = new Dictionary<string, string>() {{ "Feature1.feature", validSpecFlowFeatureFile } };
            
            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<SuperTestLibrary.Models.SpecFlowFeatureFileResponse>>>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<int>()))
                .ReturnsAsync(new SuperTestLibrary.Models.SpecFlowFeatureFileResponse()
                {
                    FeatureFiles = featureFiles,
                    GherkinDocuments = GetGherkinDocuments.ConvertSpecFlowFeatureFileResponse(new SuperTestLibrary.Models.SpecFlowFeatureFileResponse() { FeatureFiles = featureFiles} )
                });

            // Act
            var result = await _service.GenerateSpecFlowFeatureFilesAsync(selectedLlmString, requirements, cancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.FeatureFiles.Count, Is.EqualTo(1));
                Assert.That(result.FeatureFiles.First().FeatureFileName, Is.EqualTo("Feature1.feature"));
                Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Information, "Feature file generated."), Is.True);
            });
        }

        [Test]
        public void GenerateSpecFlowFeatureFilesAsync_Cancellation_ShouldLogWarning()
        {
            // Arrange
            const string requirements = "Test requirements";
            const string selectedLlmString = "GPT-4o";
            var cancellationTokenSource = new CancellationTokenSource();

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<SuperTestLibrary.Models.SpecFlowFeatureFileResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await _service.GenerateSpecFlowFeatureFilesAsync(selectedLlmString, requirements, cancellationTokenSource.Token));

            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Warning, "Operation was cancelled."), Is.True);
        }

        [Test]
        public void GenerateSpecFlowFeatureFilesAsync_Exception_ShouldLogError()
        {
            // Arrange
            const string requirements = "Test requirements";
            const string selectedLlmString = "GPT-4o";
            var cancellationToken = CancellationToken.None;

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<SuperTestLibrary.Models.SpecFlowFeatureFileResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await _service.GenerateSpecFlowFeatureFilesAsync(selectedLlmString, requirements, cancellationToken));

            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Error, "Exception while generating SpecFlow feature file."), Is.True);
        }

        [Test]
        public async Task GenerateSpecFlowFeatureFilesAsync_EmptyResponse_ShouldLogWarning()
        {
            // Arrange
            const string requirements = "Test requirements";
            const string selectedLlmString = "GPT-4o";
            var cancellationToken = CancellationToken.None;

            var featureFiles = new Dictionary<string, string>() { { "Feature1.feature", validSpecFlowFeatureFile } };

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<SuperTestLibrary.Models.SpecFlowFeatureFileResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ReturnsAsync(new SuperTestLibrary.Models.SpecFlowFeatureFileResponse()
                {
                    FeatureFiles = featureFiles,
                    GherkinDocuments = [null]
                });

            // Act
            var result = await _service.GenerateSpecFlowFeatureFilesAsync(selectedLlmString, requirements, cancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.FeatureFiles, Is.Empty);
                Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Warning, "Feature file is empty. Failed to generate feature file."), Is.True);
            });
        }
    }
}
