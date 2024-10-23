namespace SuperTestLibrary.LLMs
{
    public interface ILargeLanguageModel
    {
        string Id { get; }
        Task<string> Call(IEnumerable<string> messages);
    }
}
