namespace SuperTestLibrary.LLMs
{
    public interface ILargeLanguageModel
    {
        Task<string> GenerateSpecFlowFeatureFileAsync(string requirements);
    }
}
