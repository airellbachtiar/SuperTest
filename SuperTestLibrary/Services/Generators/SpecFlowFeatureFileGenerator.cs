using SuperTestLibrary.Helpers;
using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services.Prompts.Builders;

namespace SuperTestLibrary.Services.Generators
{
    public class SpecFlowFeatureFileGenerator : IGenerator
    {
        private ILargeLanguageModel? _llm;
        private string _requirements = string.Empty;

        private const string _jsonPromptClaude_3_5_Sonnet = "Services/Prompts/GenerateSpecFlowFeatureFile/Claude_3_5_Sonnet.json";
        private const string _jsonPromptGPT_4o = "Services/Prompts/GenerateSpecFlowFeatureFile/GPT_4o.json";
        private const string _jsonPromptGemini_1_5 = "Services/Prompts/GenerateSpecFlowFeatureFile/Gemini_1_5.json";

        public async Task<string> GenerateAsync(ILargeLanguageModel largeLanguageModel, string requirements)
        {
            _llm = largeLanguageModel;
            _requirements = requirements;

            string jsonPromptPath = _llm.Id switch
            {
                "Claude 3.5 Sonnet" => _jsonPromptClaude_3_5_Sonnet,
                "GPT-4o" => _jsonPromptGPT_4o,
                "Gemini 1.5" => throw new InvalidOperationException("Gemini 1.5 does not support generating SpecFlow feature files."),
                _ => throw new InvalidOperationException("Unknown LLM."),
            };

            IEnumerable<string> prompts = SetupPrompt(jsonPromptPath);
            string response = await _llm.Call(prompts);
            return response;
        }

        private IEnumerable<string> SetupPrompt(string jsonPromptPath)
        {
            var prompt = GetPrompt.ConvertJson(jsonPromptPath);

            var prompts = new SpecFlowFeatureFilePromptBuilder(_requirements).BuildPrompt(prompt);

            return prompts;
        }
    }
}
