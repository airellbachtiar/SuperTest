using SuperTestLibrary.Helpers;
using SuperTestLibrary.Services.Prompts.Builders;

namespace SuperTestLibrary.Services.Generators
{
    public class SpecFlowFeatureFileGenerator : GeneratorBase
    {
        public string? Requirements { get; set; }

        protected override string _jsonPromptClaude_3_5_Sonnet => "Services/Prompts/GenerateSpecFlowFeatureFile/Claude_3_5_Sonnet.json";
        protected override string _jsonPromptGPT_4o => "Services/Prompts/GenerateSpecFlowFeatureFile/GPT_4o.json";
        protected override string _jsonPromptGemini_1_5 => "Services/Prompts/GenerateSpecFlowFeatureFile/Gemini_1_5.json";

        protected override IEnumerable<string> SetupPrompt(string jsonPromptPath)
        {
            var prompt = GetPromptFromJson.ConvertJson(jsonPromptPath);

            var prompts = new SpecFlowFeatureFilePromptBuilder(Requirements!).BuildPrompt(prompt);

            return prompts;
        }
    }
}
