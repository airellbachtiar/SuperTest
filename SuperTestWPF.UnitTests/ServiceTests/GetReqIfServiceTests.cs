using Moq;
using SuperTestLibrary;
using SuperTestWPF.Services;

namespace SuperTestWPF.UnitTests.ServiceTests
{
    public class GetReqIfServiceTests
    {
        private Mock<ISuperTestController> _mockController;
        private GetReqIfService _service;

        [SetUp]
        public void SetUp()
        {
            _mockController = new Mock<ISuperTestController>();
            _service = new GetReqIfService(_mockController.Object);
        }

        [Test]
        public async Task GetAll_ShouldReturnListOfFiles()
        {
            // Arrange
            var mockFiles = new List<string> { "File1.reqif", "File2.reqif" };

            _mockController
                .Setup(c => c.GetAllReqIFFilesAsync())
                .ReturnsAsync(mockFiles);

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.That(result, Is.EqualTo(mockFiles));
            _mockController.Verify(c => c.GetAllReqIFFilesAsync(), Times.Once);
        }

        [Test]
        public void RequirementsStorageLocation_Get_ShouldReturnStorageLocation()
        {
            // Arrange
            const string mockLocation = "C:\\Requirements";

            _mockController
                .Setup(c => c.GetStorageLocation())
                .Returns(mockLocation);

            // Act
            var result = _service.RequirementsStorageLocation;

            // Assert
            Assert.That(result, Is.EqualTo(mockLocation));
            _mockController.Verify(c => c.GetStorageLocation(), Times.Once);
        }

        [Test]
        public void RequirementsStorageLocation_Set_ShouldUpdateStorageLocation()
        {
            // Arrange
            const string newLocation = "D:\\NewRequirements";

            // Act
            _service.RequirementsStorageLocation = newLocation;

            // Assert
            _mockController.Verify(c => c.UpdateStorageLocation(newLocation), Times.Once);
        }
    }
}
