using Moq;
using SuperTestWPF.Services;
using SuperTestWPF.ViewModels;

namespace SuperTestWPF.UnitTests.ViewModelTests
{
    public class SettingsWindowViewModelTests
    {
        private Mock<IGetReqIfService> _mockGetReqIfService;
        private Mock<IFileService> _mockFileService;
        private SettingsWindowViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _mockGetReqIfService = new Mock<IGetReqIfService>();
            _mockFileService = new Mock<IFileService>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(sp => sp.GetService(typeof(IGetReqIfService))).Returns(_mockGetReqIfService.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(IFileService))).Returns(_mockFileService.Object);

            _mockGetReqIfService.SetupGet(s => s.RequirementsStorageLocation).Returns("C:\\DefaultPath");

            _viewModel = new SettingsWindowViewModel(serviceProvider.Object);
        }

        [Test]
        public void ViewModel_ShouldInitializeWithDefaultSavePath()
        {
            // Arrange & Act
            var initialPath = _viewModel.SavePath;

            // Assert
            Assert.That(initialPath, Is.EqualTo("C:\\DefaultPath"));
        }

        [Test]
        public void SelectSaveRequirementFilesLocation_ShouldUpdateSavePath()
        {
            // Arrange
            const string newPath = "C:\\NewPath";
            _mockFileService.Setup(fs => fs.SelectFolderLocation(It.IsAny<string>())).Returns(newPath);

            // Act
            _viewModel.SelectSaveRequirementFilesLocationCommand.Execute(null);

            // Assert
            Assert.That(_viewModel.SavePath, Is.EqualTo(newPath));
        }

        [Test]
        public void SelectSaveRequirementFilesLocation_ShouldClearSettingStatus()
        {
            // Arrange
            _viewModel.SettingStatus = "Previous Status";

            // Act
            _viewModel.SelectSaveRequirementFilesLocationCommand.Execute(null);

            // Assert
            Assert.That(_viewModel.SettingStatus, Is.Empty);
        }

        [Test]
        public void ApplyNewRequirementFilesLocation_ShouldUpdateRequirementsStorageLocation()
        {
            // Arrange
            const string newPath = "C:\\NewPath";
            _viewModel.SavePath = newPath;

            // Act
            _viewModel.ApplyNewRequirementFilesLocationCommand.Execute(null);

            // Assert
            _mockGetReqIfService.VerifySet(s => s.RequirementsStorageLocation = newPath, Times.Once);
        }

        [Test]
        public void ApplyNewRequirementFilesLocation_ShouldUpdateSettingStatus()
        {
            // Act
            _viewModel.ApplyNewRequirementFilesLocationCommand.Execute(null);

            // Assert
            Assert.That(_viewModel.SettingStatus, Is.EqualTo("Settings applied"));
        }

        [Test]
        public void SavePath_Setter_ShouldRaisePropertyChanged()
        {
            // Arrange
            var wasRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SettingsWindowViewModel.SavePath))
                {
                    wasRaised = true;
                }
            };

            // Act
            _viewModel.SavePath = "C:\\AnotherPath";

            // Assert
            Assert.That(wasRaised, Is.True);
        }

        [Test]
        public void SettingStatus_Setter_ShouldRaisePropertyChanged()
        {
            // Arrange
            var wasRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SettingsWindowViewModel.SettingStatus))
                {
                    wasRaised = true;
                }
            };

            // Act
            _viewModel.SettingStatus = "New Status";

            // Assert
            Assert.That(wasRaised, Is.True);
        }
    }
}
