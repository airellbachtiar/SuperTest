namespace SuperTestLibrary.LLMs
{
    public interface ILLM
    {
        Task<string> GenerateSpecFlowFeatureFileAsync(string requirement);
    }
}
