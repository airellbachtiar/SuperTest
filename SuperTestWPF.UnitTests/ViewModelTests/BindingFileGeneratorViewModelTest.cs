using Microsoft.Extensions.Logging;
using Moq;
using SuperTestWPF.Models;
using SuperTestWPF.Services;
using SuperTestWPF.UnitTests.Helper;
using SuperTestWPF.ViewModels;
using System.Collections.ObjectModel;

namespace SuperTestWPF.UnitTests.ViewModelTests
{
    public class BindingFileGeneratorViewModelTest
    {
        public class BindingFileGeneratorViewModelTests
        {
            private Mock<ILogger<BindingFileGeneratorViewModel>> _mockLogger;
            private Mock<ILoggerFactory> _mockLoggerFactory;
            private Mock<IFileService> _mockFileService;
            private Mock<IBindingFileGeneratorService> _mockBindingFileGeneratorService;
            private Mock<IPromptVerboseService> _mockPromptVerboseService;
            private BindingFileGeneratorViewModel _viewModel;

            [SetUp]
            public void SetUp()
            {
                _mockLogger = new Mock<ILogger<BindingFileGeneratorViewModel>>();
                _mockLoggerFactory = new Mock<ILoggerFactory>();
                _mockFileService = new Mock<IFileService>();
                _mockBindingFileGeneratorService = new Mock<IBindingFileGeneratorService>();
                _mockPromptVerboseService = new Mock<IPromptVerboseService>();

                var serviceProvider = new Mock<IServiceProvider>();
                serviceProvider.Setup(sp => sp.GetService(typeof(ILogger<BindingFileGeneratorViewModel>))).Returns(_mockLogger.Object);
                serviceProvider.Setup(sp => sp.GetService(typeof(ILoggerFactory))).Returns(_mockLoggerFactory.Object);
                serviceProvider.Setup(sp => sp.GetService(typeof(IFileService))).Returns(_mockFileService.Object);
                serviceProvider.Setup(sp => sp.GetService(typeof(IBindingFileGeneratorService))).Returns(_mockBindingFileGeneratorService.Object);
                serviceProvider.Setup(sp => sp.GetService(typeof(IPromptVerboseService))).Returns(_mockPromptVerboseService.Object);

                _viewModel = new BindingFileGeneratorViewModel(serviceProvider.Object);
            }

            [Test]
            public void ViewModel_ShouldInitializeCommands()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(_viewModel.GenerateBindingsCommand.CanExecute(null), Is.True);
                    Assert.That(_viewModel.UploadFilesCommand.CanExecute(null), Is.True);
                    Assert.That(_viewModel.UploadFeatureFileCommand.CanExecute(null), Is.True);
                    Assert.That(_viewModel.ClearAllUploadedFilesCommand.CanExecute(null), Is.True);
                    Assert.That(_viewModel.SaveBindingFileCommand.CanExecute(null), Is.True);
                });
            }

            [Test]
            public async Task GenerateBindings_NoFeatureFile_ShouldLogWarningAndError()
            {
                // Arrange
                _viewModel.UploadedFeatureFile = null;

                // Act
                await _viewModel.GenerateBindings();

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Warning, "No feature file uploaded."), Is.True);
                    Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Error, "Failed to generate binding file."), Is.True);
                    Assert.That(_viewModel.GeneratedBindingFile, Is.Empty);
                });
            }

            [Test]
            public async Task GenerateBindings_ValidFeatureFile_ShouldPopulateBindingFiles()
            {
                // Arrange
                var testFile = new FileInformation("Test.feature", "Feature content");
                _viewModel.UploadedFeatureFile = testFile;
                _viewModel.UploadedFiles = new ObservableCollection<FileInformation>
                {
                    new FileInformation("File1.txt", "Content1"),
                    new FileInformation("File2.txt", "Content2")
                };

                    var bindingFileModels = new List<SpecFlowBindingFileModel>
                {
                    new("TestBinding.cs", "Generated content" )
                };

                    var prompts = new List<PromptHistory>
                {
                    new(DateTime.Now, "BindingFileGenerator", "GPT-4o", "test prompt")
                };

                _mockBindingFileGeneratorService
                    .Setup(s => s.GenerateBindingFilesAsync(
                        _viewModel.SelectedLLM,
                        testFile,
                        _viewModel.UploadedFiles,
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new SpecFlowBindingFileResponse(bindingFileModels, prompts));

                // Act
                await _viewModel.GenerateBindings();

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(_viewModel.SpecFlowBindingFiles.Count, Is.EqualTo(1));
                    Assert.That(_viewModel.GeneratedBindingFile, Is.EqualTo("Generated content"));
                    _mockPromptVerboseService.Verify(p => p.AddPrompt(It.IsAny<PromptHistory>()), Times.Exactly(prompts.Count));
                });
            }

            [Test]
            public void UploadFiles_ShouldAddUploadedFiles()
            {
                // Arrange
                var mockFiles = new Dictionary<string, string>
                {
                    { "File1.txt", "Content1" },
                    { "File2.txt", "Content2" }
                };

                _mockFileService.Setup(fs => fs.OpenFilesDialog(It.IsAny<string>())).Returns(mockFiles.Keys.ToList());
                foreach (var file in mockFiles)
                {
                    _mockFileService.Setup(fs => fs.GetFileContent(file.Key)).Returns(file.Value);
                }

                // Act
                _viewModel.UploadFiles();

                // Assert
                Assert.That(_viewModel.UploadedFiles.Count, Is.EqualTo(2));
                Assert.That(_viewModel.UploadedFiles.First().Path!.Contains("File1.txt"));
                Assert.That(_viewModel.UploadedFiles.Last().Path!.Contains("File2.txt"));
            }

            [Test]
            public void UploadFeatureFile_ShouldSetUploadedFeatureFile()
            {
                // Arrange
                const string testFilePath = "C:\\Test\\TestFeature.feature";
                const string testContent = "Feature file content";

                _mockFileService.Setup(fs => fs.OpenFileDialog(It.IsAny<string>())).Returns(testFilePath);
                _mockFileService.Setup(fs => fs.GetFileContent(testFilePath)).Returns(testContent);

                // Act
                _viewModel.UploadFeatureFile();

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(_viewModel.UploadedFeatureFile, Is.Not.Null);
                    Assert.That(_viewModel.UploadedFeatureFile!.Path, Is.EqualTo("TestFeature.feature"));
                    Assert.That(_viewModel.UploadedFeatureFile.Value, Is.EqualTo(testContent));
                    Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Information, "Feature file uploaded."), Is.True);
                });
            }

            [Test]
            public void ClearAllUploadedFiles_ShouldClearUploadedFiles()
            {
                // Arrange
                _viewModel.UploadedFiles.Add(new FileInformation("File1.txt", "Content1"));
                _viewModel.UploadedFeatureFile = new FileInformation("Feature.feature", "Feature content");

                // Act
                _viewModel.ClearAllUploadedFilesCommand.Execute(null);

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(_viewModel.UploadedFiles, Is.Empty);
                    Assert.That(_viewModel.UploadedFeatureFile, Is.Null);
                });
            }

            [Test]
            public void SaveBindingFile_NoBindingFile_ShouldLogWarningAndError()
            {
                // Arrange
                _viewModel.GeneratedBindingFile = string.Empty;

                // Act
                _viewModel.SaveBindingFile();

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Warning, "No binding file generated."), Is.True);
                    Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Error, "Failed to save binding file."), Is.True);
                });
            }

            [Test]
            public void SaveBindingFile_ValidBindingFile_ShouldSaveFile()
            {
                // Arrange
                _viewModel.GeneratedBindingFile = "Generated binding content";
                _viewModel.SavePath = "C:\\Test\\SaveLocation";

                _viewModel.SpecFlowBindingFiles.Add(new SpecFlowBindingFileModel("TestBinding.cs", "Generated binding content"));

                // Act
                _viewModel.SaveBindingFile();

                // Assert
                _mockFileService.Verify(fs => fs.SaveFile(
                    It.Is<string>(path => path.EndsWith("TestBinding.cs")),
                    It.Is<string>(content => content == "Generated binding content")),
                    Times.Once);
            }
        }
    }
}
