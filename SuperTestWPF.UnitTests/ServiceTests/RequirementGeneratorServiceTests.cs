using Microsoft.Extensions.Logging;
using Moq;
using SuperTestLibrary;
using SuperTestLibrary.Models;
using SuperTestWPF.Retry;
using SuperTestWPF.Services;
using SuperTestWPF.UnitTests.Helper;

namespace SuperTestWPF.UnitTests.ServiceTests
{
    public class RequirementGeneratorServiceTests
    {
        private Mock<ISuperTestController> _mockController;
        private Mock<ILogger<RequirementGeneratorService>> _mockLogger;
        private Mock<IRetryService> _mockRetry;
        private RequirementGeneratorService _service;

        [SetUp]
        public void SetUp()
        {
            _mockController = new Mock<ISuperTestController>();
            _mockLogger = new Mock<ILogger<RequirementGeneratorService>>();
            _mockRetry = new Mock<IRetryService>();

            _service = new RequirementGeneratorService(_mockController.Object, _mockLogger.Object, _mockRetry.Object);
        }

        [Test]
        public async Task GenerateRequirementAsync_ValidInputs_ShouldReturnRequirementResponse()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            const string existingRequirement = "Existing requirement";

            var testFiles = new Dictionary<string, string>
            {
                { "File1.txt", "Test content 1" },
                { "File2.txt", "Test content 2" }
            };

            var generatedResponse = new RequirementResponse
            {
                Response = "Generated requirement",
                Requirements = new List<RequirementModel>
                {
                    new() { Id = "1", Content = "Content1", Trace = "Trace1" },
                    new() { Id = "2", Content = "Content2", Trace = "Trace2" }
                },
                Prompts = ["Prompt1", "Prompt2"]
            };

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<RequirementResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ReturnsAsync(generatedResponse);

            // Act
            var result = await _service.GenerateRequirementAsync(selectedLlmString, testFiles, existingRequirement, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Response, Is.EqualTo("Generated requirement"));
                Assert.That(result.Prompts.Count(), Is.EqualTo(2));
                Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Information, "Generating requirement..."), Is.True);
            });
        }

        [Test]
        public void GenerateRequirementAsync_Cancellation_ShouldLogWarning()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            const string existingRequirement = "Existing requirement";

            var testFiles = new Dictionary<string, string>();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<RequirementResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await _service.GenerateRequirementAsync(selectedLlmString, testFiles, existingRequirement, cancellationTokenSource.Token));
            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Warning, "Operation was cancelled."), Is.True);
        }

        [Test]
        public void GenerateRequirementAsync_Exception_ShouldLogError()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            const string existingRequirement = "Existing requirement";

            var testFiles = new Dictionary<string, string>();

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<RequirementResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));

            _mockController
                .Setup(c => c.SelectedLLM!.Id)
                .Returns(selectedLlmString);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await _service.GenerateRequirementAsync(selectedLlmString, testFiles, existingRequirement, CancellationToken.None));
            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Error, $"Exception while generating requirement using {selectedLlmString}"), Is.True);
        }

        [Test]
        public async Task GenerateRequirementAsync_EmptyResponse_ShouldReturnEmptyRequirement()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            const string existingRequirement = "Existing requirement";

            var testFiles = new Dictionary<string, string>();

            var generatedResponse = new RequirementResponse
            {
                Response = string.Empty,
                Requirements = [],
                Prompts = []
            };

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<RequirementResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ReturnsAsync(generatedResponse);

            // Act
            var result = await _service.GenerateRequirementAsync(selectedLlmString, testFiles, existingRequirement, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Response, Is.Empty);
                Assert.That(result.Requirements, Is.Empty);
                Assert.That(result.Prompts, Is.Empty);
            });
        }
    }
}
