namespace SuperTestLibrary.LLMs
{
    public interface ILargeLanguageModel
    {
        string Id { get; }
        Task<string> CallAsync(IEnumerable<string> messages);
    }
}
