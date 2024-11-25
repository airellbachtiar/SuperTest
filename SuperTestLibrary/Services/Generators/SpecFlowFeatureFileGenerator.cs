using SuperTestLibrary.Helpers;
using SuperTestLibrary.Services.Prompts.Builders;

namespace SuperTestLibrary.Services.Generators
{
    public class SpecFlowFeatureFileGenerator : GeneratorBase
    {
        public string? Requirements { get; set; }

        protected override string JsonPromptClaude35Sonnet => "Services/Prompts/GenerateSpecFlowFeatureFile/Claude_3_5_Sonnet.json";
        protected override string JsonPromptGPT4o => "Services/Prompts/GenerateSpecFlowFeatureFile/GPT_4o.json";
        protected override string JsonPromptGemini15 => "Services/Prompts/GenerateSpecFlowFeatureFile/Gemini_1_5.json";

        protected override IEnumerable<string> SetupPrompt(string jsonPromptPath)
        {
            var prompt = GetPromptFromJson.ConvertJson(jsonPromptPath);

            var prompts = new SpecFlowFeatureFilePromptBuilder(Requirements!).BuildPrompt(prompt);

            return prompts;
        }
    }
}
