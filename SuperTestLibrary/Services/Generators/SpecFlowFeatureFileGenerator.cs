using LargeLanguageModelLibrary.Models;
using SuperTestLibrary.Helpers;
using SuperTestLibrary.Services.PromptBuilders;

namespace SuperTestLibrary.Services.Generators
{
    public class SpecFlowFeatureFileGenerator : GeneratorBase
    {
        public string? Requirements { get; set; }

        protected override string JsonPromptClaude35Sonnet => "Prompts/GenerateSpecFlowFeatureFile/Claude_3_5_Sonnet.json";
        protected override string JsonPromptGPT4o => "Prompts/GenerateSpecFlowFeatureFile/GPT_4o.json";
        protected override string JsonPromptGemini15 => "Prompts/GenerateSpecFlowFeatureFile/Gemini_1_5.json";

        protected override MessageRequest SetupPrompt(string jsonPromptPath)
        {
            var prompt = GetPromptFromJson.ConvertJson(jsonPromptPath);

            var request = new SpecFlowFeatureFilePromptBuilder(Requirements!).BuildPrompt(prompt);

            return request;
        }
    }
}
