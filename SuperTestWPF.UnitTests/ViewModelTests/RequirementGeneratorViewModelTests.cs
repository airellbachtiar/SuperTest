using Microsoft.Extensions.Logging;
using Moq;
using ReqIFSharp;
using SuperTestWPF.Models;
using SuperTestWPF.Services;
using SuperTestWPF.UnitTests.Helper;
using SuperTestWPF.ViewModels;
using System.Collections.ObjectModel;

namespace SuperTestWPF.UnitTests.ViewModelTests
{
    public class RequirementGeneratorViewModelTests
    {
        private Mock<IFileService> _mockFileService;
        private Mock<IRequirementGeneratorService> _mockRequirementGeneratorService;
        private Mock<IPromptVerboseService> _mockPromptVerboseService;
        private Mock<IReqIFConverterService> _mockReqIFConverterService;
        private Mock<ILogger<MainWindowViewModel>> _mockLogger;
        private Mock<ILoggerFactory> _mockLoggerFactory;
        private RequirementGeneratorViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _mockFileService = new Mock<IFileService>();
            _mockRequirementGeneratorService = new Mock<IRequirementGeneratorService>();
            _mockPromptVerboseService = new Mock<IPromptVerboseService>();
            _mockReqIFConverterService = new Mock<IReqIFConverterService>();
            _mockLogger = new Mock<ILogger<MainWindowViewModel>>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(sp => sp.GetService(typeof(IFileService))).Returns(_mockFileService.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(IRequirementGeneratorService))).Returns(_mockRequirementGeneratorService.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(IPromptVerboseService))).Returns(_mockPromptVerboseService.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(IReqIFConverterService))).Returns(_mockReqIFConverterService.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(ILogger<MainWindowViewModel>))).Returns(_mockLogger.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(ILoggerFactory))).Returns(_mockLoggerFactory.Object);

            _viewModel = new RequirementGeneratorViewModel(serviceProvider.Object);
        }

        [Test]
        public void UploadTestFiles_ShouldAddUploadedFiles()
        {
            // Arrange
            var mockFiles = new Dictionary<string, string>
            {
                { "File1.txt", "Content1" },
                { "File2.txt", "Content2" }
            };

            _mockFileService.Setup(fs => fs.OpenFilesDialog(It.IsAny<string>())).Returns([.. mockFiles.Keys]);
            foreach (var file in mockFiles)
            {
                _mockFileService.Setup(fs => fs.GetFileContent(file.Key)).Returns(file.Value);
            }

            // Act
            _viewModel.UploadTestFilesCommand.Execute(null);

            // Assert
            Assert.That(_viewModel.UploadedTestFiles.Count, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(_viewModel.UploadedTestFiles.First().Path, Does.Contain("File1.txt"));
                Assert.That(_viewModel.UploadedTestFiles.Last().Path, Does.Contain("File2.txt"));
            });
        }

        [Test]
        public async Task GenerateRequirement_NoFilesUploaded_ShouldLogWarningAndError()
        {
            // Arrange
            _viewModel.UploadedTestFiles.Clear();

            // Act
            await _viewModel.GenerateRequirement();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Warning, "No test files uploaded."), Is.True);
                Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Error, "Failed to generate requirement."), Is.True);
            });
        }

        [Test]
        public async Task GenerateRequirement_WithUploadedFiles_ShouldPopulateGeneratedRequirements()
        {
            // Arrange
            var testFiles = new ObservableCollection<FileInformation>
            {
                new("File1.txt", "Content1"),
                new("File2.txt", "Content2")
            };

            var generatedRequirements = new List<RequirementModel>
            {
                new() { Content = "Requirement1" },
                new() { Content = "Requirement2" }
            };

            var prompts = new List<PromptHistory>
            {
                new(DateTime.Now, "RequirementGenerator", "GPT-4o", "test prompt")
            };

            _viewModel.UploadedTestFiles = testFiles;
            _mockRequirementGeneratorService
                .Setup(s => s.GenerateRequirementAsync(
                    _viewModel.SelectedLLM,
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RequirementResponse("Requirement", generatedRequirements, prompts));

            // Act
            await _viewModel.GenerateRequirement();

            // Assert
            Assert.That(_viewModel.GeneratedRequirements.Count, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(_viewModel.GeneratedRequirements.First().Content, Is.EqualTo("Requirement1"));
                Assert.That(_viewModel.GeneratedRequirements.Last().Content, Is.EqualTo("Requirement2"));
            });
            _mockPromptVerboseService.Verify(p => p.AddPrompt(It.IsAny<PromptHistory>()), Times.Exactly(prompts.Count));
        }

        [Test]
        public void SaveRequirementFiles_NoRequirementsGenerated_ShouldLogWarningAndError()
        {
            // Arrange
            _viewModel.GeneratedRequirements.Clear();

            // Act
            _viewModel.SaveRequirementFilesCommand.Execute(null);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Warning, "No requirements generated."), Is.True);
                Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Error, "Failed to save requirements."), Is.True);
            });
        }

        [Test]
        public void SaveRequirementFiles_WithGeneratedRequirements_ShouldSaveRequirements()
        {
            // Arrange
            _viewModel.SavePath = "C:\\Test\\SaveLocation";

            var requirements = new ObservableCollection<RequirementModel>
            {
                new() {Id = "01", Content = "Requirement1" },
                new() {Id = "02", Content = "Requirement2" }
            };

            _viewModel.GeneratedRequirements = requirements;
            _mockReqIFConverterService.Setup(s => s.ConvertRequirementToReqIfAsync(requirements))
                .Returns(new ReqIF());

            // Act
            _viewModel.SaveRequirementFilesCommand.Execute(null);

            // Assert
            _mockFileService.Verify(fs => fs.SaveFile(
                It.Is<string>(path => path.EndsWith("requirements.reqif")),
                It.Is<ReqIF>(content => content != null)),
                Times.Once);
            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Information, "Requirements saved."), Is.True);
        }

        [Test]
        public void SelectSaveLocation_ShouldUpdateSavePath()
        {
            // Arrange
            const string newPath = "C:\\NewPath";
            _mockFileService.Setup(fs => fs.SelectFolderLocation(It.IsAny<string>())).Returns(newPath);

            // Act
            _viewModel.SelectSaveLocationCommand.Execute(null);

            // Assert
            Assert.That(_viewModel.SavePath, Is.EqualTo(newPath));
        }
    }
}
