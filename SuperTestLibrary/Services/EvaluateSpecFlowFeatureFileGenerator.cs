using SuperTestLibrary.Helpers;
using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services.Prompts.Builders;

namespace SuperTestLibrary.Services
{
    public class EvaluateSpecFlowFeatureFileGenerator : IGenerator
    {
        private ILargeLanguageModel? _llm;
        private readonly string _requirements;
        private string _featureFile = string.Empty;

        private const string _jsonPromptClaude_3_5_Sonnet = "Services/Prompts/EvaluateSpecFlowFeatureFile/Claude_3_5_Sonnet.json";
        private const string _jsonPromptGPT_4o = "Services/Prompts/EvaluateSpecFlowFeatureFile/GPT_4o.json";

        public EvaluateSpecFlowFeatureFileGenerator(string requirements)
        {
            _requirements = requirements;
        }

        public async Task<string> Generate(ILargeLanguageModel largeLanguageModel, string featureFile)
        {
            _llm = largeLanguageModel;
            _featureFile = featureFile;

            string jsonPromptPath = string.Empty;
            if (!string.IsNullOrEmpty(_requirements)) {
                jsonPromptPath = _llm.Id switch
                {
                    "Claude 3.5 Sonnet" => _jsonPromptClaude_3_5_Sonnet,
                    "GPT-4o" => _jsonPromptGPT_4o,
                    _ => throw new InvalidOperationException("Unknown LLM."),
                };
            }
            else
            {
                throw new InvalidOperationException("No requirements provided.");
            }

            IEnumerable<string> prompts = SetupPrompt(jsonPromptPath);
            return await _llm.Call(prompts);
        }

        private IEnumerable<string> SetupPrompt(string jsonPromptPath)
        {
            var prompt = GetPrompt.ConvertJson(jsonPromptPath);

            var prompts = new EvaluateSpecFlowFeatureFilePromptBuilder(_requirements, _featureFile).BuildPrompt(prompt);

            return prompts;
        }
    }
}
