namespace SuperTestWPF.Retry
{
    public interface IRetryService
    {
        Task DoAsync(Func<Task> action, TimeSpan retryInterval, int maxAttemptCount = 3);
        Task<T> DoAsync<T>(Func<Task<T>> action, TimeSpan retryInterval, int maxAttemptCount = 3);
    }
}