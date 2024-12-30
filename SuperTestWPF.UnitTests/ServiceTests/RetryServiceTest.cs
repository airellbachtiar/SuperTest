using Microsoft.Extensions.Logging;
using Moq;
using SuperTestWPF.Retry;

namespace SuperTestWPF.UnitTests.ServiceTests
{
    public class RetryServiceTests
    {
        private Mock<ILogger<RetryService>> _mockLogger;
        private RetryService _retryService;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<RetryService>>();
            _retryService = new RetryService(_mockLogger.Object);
        }

        [Test]
        public async Task DoAsync_ShouldSucceedOnFirstAttempt()
        {
            // Arrange
            var attemptCount = 0;
            Task TestAction()
            {
                attemptCount++;
                return Task.CompletedTask;
            }

            // Act
            await _retryService.DoAsync(TestAction, TimeSpan.FromMilliseconds(100));

            // Assert
            Assert.That(attemptCount, Is.EqualTo(1));
            _mockLogger.Verify(log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Never);
        }

        [Test]
        public async Task DoAsync_ShouldRetryOnFailure()
        {
            // Arrange
            var attemptCount = 0;
            Task TestAction()
            {
                attemptCount++;
                if (attemptCount < 3)
                {
                    throw new Exception("Test exception");
                }

                return Task.CompletedTask;
            }

            // Act
            await _retryService.DoAsync(TestAction, TimeSpan.FromMilliseconds(100), maxAttemptCount: 3);

            // Assert
            Assert.That(attemptCount, Is.EqualTo(3));
            _mockLogger.Verify(log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Attempt")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Exactly(2));
        }

        [Test]
        public void DoAsync_ShouldThrowAggregateExceptionAfterMaxAttempts()
        {
            // Arrange
            static Task TestAction()
            {
                throw new Exception("Test exception");
            }

            // Act & Assert
            var ex = Assert.ThrowsAsync<AggregateException>(async () =>
                await _retryService.DoAsync(TestAction, TimeSpan.FromMilliseconds(100), maxAttemptCount: 3));

            Assert.That(ex.InnerExceptions.Count, Is.EqualTo(3));
            _mockLogger.Verify(log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("All attempts failed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Once);
        }

        [Test]
        public void DoAsync_ShouldThrowOperationCanceledExceptionIfCancelled()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            async Task TestAction()
            {
                await Task.Delay(100, cts.Token);
            }

            // Act & Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await _retryService.DoAsync(TestAction, TimeSpan.FromMilliseconds(100), maxAttemptCount: 3));

            _mockLogger.Verify(log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Operation was cancelled.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Once);
        }

        [Test]
        public async Task DoAsync_ShouldDelayBetweenRetries()
        {
            // Arrange
            var timestamps = new List<DateTime>();

            Task TestAction()
            {
                timestamps.Add(DateTime.UtcNow);
                throw new Exception("Test exception");
            }

            // Act
            try
            {
                await _retryService.DoAsync(TestAction, TimeSpan.FromMilliseconds(200), maxAttemptCount: 3);
            }
            catch
            {
                // Expected
            }

            // Assert
            Assert.That(timestamps, Has.Count.EqualTo(3));
            var delays = timestamps.Zip(timestamps.Skip(1), (first, second) => (second - first).TotalMilliseconds);
            foreach (var delay in delays)
            {
                Assert.That(delay, Is.GreaterThanOrEqualTo(200));
            }
        }
    }
}
