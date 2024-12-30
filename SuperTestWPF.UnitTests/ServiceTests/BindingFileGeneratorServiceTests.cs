using LlmLibrary.Models;
using Microsoft.Extensions.Logging;
using Moq;
using SuperTestLibrary;
using SuperTestWPF.Models;
using SuperTestWPF.Retry;
using SuperTestWPF.Services;
using SuperTestWPF.UnitTests.Helper;
using System.Collections.ObjectModel;

namespace SuperTestWPF.UnitTests.ServiceTests
{
    public class BindingFileGeneratorServiceTests
    {
        private Mock<ISuperTestController> _mockController;
        private Mock<ILogger<BindingFileGeneratorService>> _mockLogger;
        private Mock<IRetryService> _mockRetry;
        private BindingFileGeneratorService _service;

        [SetUp]
        public void SetUp()
        {
            _mockController = new Mock<ISuperTestController>();
            _mockLogger = new Mock<ILogger<BindingFileGeneratorService>>();
            _mockRetry = new Mock<IRetryService>();

            _service = new BindingFileGeneratorService(_mockController.Object, _mockLogger.Object, _mockRetry.Object);
        }

        [Test]
        public async Task GenerateBindingFilesAsync_ValidInputs_ShouldReturnBindingFiles()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            var featureFile = new FileInformation("Feature1.feature", "Feature content");
            var additionalCode = new ObservableCollection<FileInformation>
            {
                new FileInformation("Code1.cs", "Code content 1"),
                new FileInformation("Code2.cs", "Code content 2")
            };

            var generatedFiles = new Dictionary<string, string>
            {
                { "Binding1.cs", "Binding content 1" },
                { "Binding2.cs", "Binding content 2" }
            };
            var prompts = new List<string> { "Prompt1", "Prompt2" };

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<SuperTestLibrary.Models.SpecFlowBindingFileResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ReturnsAsync(new SuperTestLibrary.Models.SpecFlowBindingFileResponse()
                {
                    BindingFiles = generatedFiles,
                    Prompts = prompts
                });

            // Act
            var result = await _service.GenerateBindingFilesAsync(selectedLlmString, featureFile, additionalCode);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.specFlowBindingFileModels.Count, Is.EqualTo(generatedFiles.Count));
                Assert.That(result.specFlowBindingFileModels.First().BindingFileName, Is.EqualTo("Binding1.cs"));
                Assert.That(result.specFlowBindingFileModels.Last().BindingFileContent, Is.EqualTo("Binding content 2"));
                Assert.That(result.Prompts.Count, Is.EqualTo(prompts.Count));
                Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Information, "Generating binding file..."), Is.True);
            });
        }

        [Test]
        public void GenerateBindingFilesAsync_Cancellation_ShouldLogWarning()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            var featureFile = new FileInformation("Feature1.feature", "Feature content");
            var additionalCode = new ObservableCollection<FileInformation>();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<SuperTestLibrary.Models.SpecFlowBindingFileResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await _service.GenerateBindingFilesAsync(selectedLlmString, featureFile, additionalCode, cancellationTokenSource.Token));
            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Warning, "Operation was cancelled."), Is.True);
        }

        [Test]
        public void GenerateBindingFilesAsync_Exception_ShouldLogError()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            var featureFile = new FileInformation("Feature1.feature", "Feature content");
            var additionalCode = new ObservableCollection<FileInformation>();

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<SuperTestLibrary.Models.SpecFlowBindingFileResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));

            _mockController
                .Setup(c => c.SelectedLLM!.Id)
                .Returns(selectedLlmString);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
                await _service.GenerateBindingFilesAsync(selectedLlmString, featureFile, additionalCode));
            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Error, $"Exception while generating binding file for {featureFile.Path} using {selectedLlmString}"), Is.True);
        }

        [Test]
        public async Task GenerateBindingFilesAsync_EmptyResponse_ShouldReturnEmptyResult()
        {
            // Arrange
            const string selectedLlmString = "GPT-4o";
            var featureFile = new FileInformation("Feature1.feature", "Feature content");
            var additionalCode = new ObservableCollection<FileInformation>();

            _mockRetry
                .Setup(r => r.DoAsync(
                    It.IsAny<Func<Task<SuperTestLibrary.Models.SpecFlowBindingFileResponse>>>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .ReturnsAsync(new SuperTestLibrary.Models.SpecFlowBindingFileResponse());

            // Act
            var result = await _service.GenerateBindingFilesAsync(selectedLlmString, featureFile, additionalCode);

            // Assert
            Assert.That(result.specFlowBindingFileModels, Is.Empty);
            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Information, "Generating binding file..."), Is.True);
        }
    }
}
