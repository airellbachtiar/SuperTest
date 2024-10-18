namespace SuperTestLibrary.LLMs.Models
{
    public class SpecFlowFeatureFileResponse
    {
        // Key: FeatureFileName, Value: FeatureFileContent
        public Dictionary<string, string> FeatureFiles { get; init; } = [];
    }
}
