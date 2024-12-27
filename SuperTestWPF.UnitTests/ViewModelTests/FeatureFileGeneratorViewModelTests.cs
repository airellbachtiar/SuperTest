using Microsoft.Extensions.Logging;
using SuperTestWPF.ViewModels;
using SuperTestWPF.Services;
using Moq;
using SuperTestWPF.Models;
using SuperTestWPF.UnitTests.Helper;

namespace SuperTestWPF.UnitTests.ViewModelTests
{
    public class FeatureFileGeneratorViewModelTests
    {
        private readonly Mock<ILogger<FeatureFileGeneratorViewModel>> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly Mock<IGetReqIfService> _mockGetReqIfService;
        private readonly Mock<IFeatureFileGeneratorService> _mockFeatureFileService;
        private readonly Mock<IFileService> _mockFileService;
        private readonly Mock<IEvaluateFeatureFileService> _mockEvaluateFeatureFileService;
        private readonly Mock<IPromptVerboseService> _mockPromptVerboseService;

        private readonly FeatureFileGeneratorViewModel _viewModel;

        public FeatureFileGeneratorViewModelTests()
        {
            _mockLogger = new Mock<ILogger<FeatureFileGeneratorViewModel>>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();
            _mockGetReqIfService = new Mock<IGetReqIfService>();
            _mockFeatureFileService = new Mock<IFeatureFileGeneratorService>();
            _mockFileService = new Mock<IFileService>();
            _mockEvaluateFeatureFileService = new Mock<IEvaluateFeatureFileService>();
            _mockPromptVerboseService = new Mock<IPromptVerboseService>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(sp => sp.GetService(typeof(ILogger<FeatureFileGeneratorViewModel>)))
                .Returns(_mockLogger.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(ILoggerFactory)))
                .Returns(_mockLoggerFactory.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(IGetReqIfService)))
                .Returns(_mockGetReqIfService.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(IFeatureFileGeneratorService)))
                .Returns(_mockFeatureFileService.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(IFileService)))
                .Returns(_mockFileService.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(IEvaluateFeatureFileService)))
                .Returns(_mockEvaluateFeatureFileService.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(IPromptVerboseService)))
                .Returns(_mockPromptVerboseService.Object);

            _viewModel = new FeatureFileGeneratorViewModel(serviceProvider.Object);
        }

        [Test]
        public void ViewModel_ShouldInitializeCommands()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_viewModel.UploadReqIFCommand.CanExecute(null), Is.True);
                Assert.That(_viewModel.GenerateAndEvaluateSpecFlowFeatureFileCommand.CanExecute(null), Is.True);
                Assert.That(_viewModel.DisplayFeatureFileScoreCommand.CanExecute(null), Is.True);
                Assert.That(_viewModel.DisplayScenarioScoreCommand.CanExecute(null), Is.True);
                Assert.That(_viewModel.SelectSaveLocationCommand.CanExecute(null), Is.True);
                Assert.That(_viewModel.SaveFeatureFilesCommand.CanExecute(null), Is.True);
                Assert.That(_viewModel.SwitchFeatureFileViewCommand.CanExecute(null), Is.True);
                Assert.That(_viewModel.OpenSettingsCommand.CanExecute(null), Is.True);
            });
        }

        [Test]
        public async Task InitializeReqIFs_ShouldLoadRequirementTitles()
        {
            // Arrange
            var mockFiles = new List<string> { "Requirement1.reqif", "Requirement2.reqif" };
            _mockGetReqIfService.Setup(s => s.GetAll()).ReturnsAsync(mockFiles);

            // Act
            await _viewModel.InitializeReqIFs();

            // Assert
            Assert.That(_viewModel.OnLoadedRequirementTitles.ToList(), Is.EqualTo(mockFiles));
        }

        [Test]
        public void UploadReqIF_ShouldLogInformation_WhenFileIsUploaded()
        {
            // Arrange
            var testFilePath = "C:\\Test\\Requirements.reqif";
            _mockFileService.Setup(fs => fs.OpenFileDialog(It.IsAny<string>())).Returns(testFilePath);

            // Act
            _viewModel.UploadReqIF();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(_viewModel.ChosenFile, Is.EqualTo(testFilePath));
                Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Information, "Uploading ReqIF..."), Is.True);
                Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Information, "ReqIF uploaded."), Is.True);
            });
        }

        [Test]
        public void SwitchFeatureFileView_ShouldToggleVisibility()
        {
            // Arrange
            var initialVisibility = _viewModel.IsFeatureFileContentVisible;

            // Act
            _viewModel.SwitchFeatureFileView();

            // Assert
            Assert.That(_viewModel.IsFeatureFileContentVisible, Is.Not.EqualTo(initialVisibility));
        }

        [Test]
        public async Task GenerateAndEvaluateSpecFlowFeatureFile_NoChosenFile_ShouldLogWarningAndError()
        {
            // Arrange
            _viewModel.ChosenFile = string.Empty;

            // Act
            await _viewModel.GenerateAndEvaluateSpecFlowFeatureFile();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Warning, "No file chosen."), Is.True);
                Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Error, "Failed to generate SpecFlow feature file."), Is.True);
                Assert.That(_viewModel.SpecFlowFeatureFiles, Is.Empty);
            });
        }

        [Test]
        public async Task GenerateAndEvaluateSpecFlowFeatureFile_ValidFile_ShouldPopulateFeatureFiles()
        {
            // Arrange
            const string testFileContent = "Requirement content";
            PromptHistory testPrompt = new(DateTime.Now, "FeatureFileGenerator", "GPT-4o", "test prompt");
            var testFeatureFiles = new List<SpecFlowFeatureFileModel>
            {
                new() { FeatureFileName = "TestFeature.feature" }
            };

            var mockResponse = new SpecFlowFeatureFileResponse(testFeatureFiles, [testPrompt]);

            _viewModel.ChosenFile = "ValidFile.reqif";
            _mockFileService.Setup(fs => fs.GetFileContent("ValidFile.reqif")).Returns(testFileContent);
            _mockFeatureFileService.Setup(fs => fs.GenerateSpecFlowFeatureFilesAsync(It.IsAny<string>(), testFileContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse);

            // Act
            await _viewModel.GenerateAndEvaluateSpecFlowFeatureFile();

            // Assert
            Assert.That(_viewModel.SpecFlowFeatureFiles, Is.Not.Empty);
            Assert.That(_viewModel.SpecFlowFeatureFiles.Count, Is.EqualTo(1));
            Assert.That(_viewModel.SpecFlowFeatureFiles[0].FeatureFileName, Is.EqualTo("TestFeature.feature"));

            _mockPromptVerboseService.Verify(pvs => pvs.AddPrompt(testPrompt), Times.Once);
        }

        [Test]
        public async Task EvaluateSpecFlowFeatureFile_NoFeatureFiles_ShouldLogWarning()
        {
            // Arrange
            _viewModel.SpecFlowFeatureFiles.Clear();

            // Act
            await _viewModel.EvaluateSpecFlowFeatureFile("Requirements content", CancellationToken.None);

            // Assert
            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Warning, "No feature file to evaluate."), Is.True);
        }

        [Test]
        public async Task EvaluateSpecFlowFeatureFile_WithFeatureFiles_ShouldEvaluateSuccessfully()
        {
            // Arrange
            PromptHistory evaluationPrompt = new(DateTime.Now, "EvaluateFeatureFileAsync", "GPT-4o", "test prompt");
            PromptHistory scenarioPrompt = new(DateTime.Now, "EvaluateSpecFlowScenarioAsync", "GPT-4o", "test prompt");
            var testFeatureFile = new SpecFlowFeatureFileModel { FeatureFileName = "TestFeature.feature" };
            _viewModel.SpecFlowFeatureFiles = [testFeatureFile];

            _mockEvaluateFeatureFileService
                .Setup(e => e.EvaluateFeatureFileAsync(It.IsAny<string>(), testFeatureFile, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([evaluationPrompt]);

            _mockEvaluateFeatureFileService
                .Setup(e => e.EvaluateSpecFlowScenarioAsync(It.IsAny<string>(), testFeatureFile, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([scenarioPrompt]);

            // Act
            await _viewModel.EvaluateSpecFlowFeatureFile("Requirements content", CancellationToken.None);

            // Assert
            _mockPromptVerboseService.Verify(pvs => pvs.AddPrompt(evaluationPrompt), Times.AtLeastOnce);
            _mockPromptVerboseService.Verify(pvs => pvs.AddPrompt(scenarioPrompt), Times.AtLeastOnce);
            Assert.That(LoggerHelper.VerifyLog(_mockLogger, LogLevel.Information, "Finished evaluating feature file!"), Is.True);
        }
    }
}