namespace LlmLibrary
{
    public interface ILargeLanguageModel
    {
        string Id { get; }
        Task<string> CallAsync(IEnumerable<string> messages, CancellationToken cancellationToken);
    }
}
