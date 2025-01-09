using Gherkin.Ast;

namespace SuperTestLibrary.Models
{
    public class SpecFlowFeatureFileResponse
    {
        // Key: FeatureFileName, Value: FeatureFileContent
        public Dictionary<string, string> FeatureFiles { get; init; } = [];
        public List<GherkinDocument> GherkinDocuments { get; set; } = [];
        public List<string> Prompts { get; set; } = [];
        public List<string> RawResponse { get; set; } = [];
    }
}
