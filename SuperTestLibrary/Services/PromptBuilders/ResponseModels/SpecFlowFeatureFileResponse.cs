using Gherkin.Ast;

namespace SuperTestLibrary.Services.PromptBuilders.ResponseModels
{
    public class SpecFlowFeatureFileResponse
    {
        // Key: FeatureFileName, Value: FeatureFileContent
        public Dictionary<string, string> FeatureFiles { get; init; } = [];
        public List<GherkinDocument?> GherkinDocuments { get; set; } = [];
        public List<string> Prompts { get; set; } = [];
    }
}
