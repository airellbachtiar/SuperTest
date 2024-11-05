using Gherkin.Ast;

namespace SuperTestLibrary.Services.Prompts.ResponseModels
{
    public class SpecFlowFeatureFileResponse
    {
        // Key: FeatureFileName, Value: FeatureFileContent
        public Dictionary<string, string> FeatureFiles { get; init; } = [];
        public GherkinDocument? GherkinDocument { get; set; } = null;
    }
}
