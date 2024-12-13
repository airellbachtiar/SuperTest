using Microsoft.Extensions.Logging;

namespace SuperTestWPF.Retry
{
    public class RetryService(ILogger<RetryService> logger) : IRetryService
    {
        private readonly ILogger<RetryService> _logger = logger;

        public async Task DoAsync(
            Func<Task> action,
            TimeSpan retryInterval,
            int maxAttemptCount = 3)
        {
            await DoAsync<object?>(async () =>
            {
                await action();
                return null;
            }, retryInterval, maxAttemptCount);
        }

        public async Task<T> DoAsync<T>(
            Func<Task<T>> action,
            TimeSpan retryInterval,
            int maxAttemptCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        await Task.Delay(retryInterval);
                    }
                    return await action();
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Operation was cancelled.");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Attempt {attempted + 1} failed.");
                    exceptions.Add(ex);
                }
            }

            _logger.LogError("All attempts failed.");
            throw new AggregateException(exceptions);
        }
    }
}
