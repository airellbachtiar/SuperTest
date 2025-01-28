using LargeLanguageModelLibrary.Models;
using SuperTestLibrary.Helpers;
using SuperTestLibrary.Services.PromptBuilders;

namespace SuperTestLibrary.Services.Generators
{
    public class RequirementGenerator : GeneratorBase
    {
        public Dictionary<string, string> TestFiles { get; set; } = [];
        public string ExistingRequirement { get; set; } = string.Empty;

        protected override string JsonPromptClaude35Sonnet => "Prompts/GenerateRequirement/RequirementPrompt.json";

        protected override string JsonPromptGPT4o => "Prompts/GenerateRequirement/RequirementPrompt.json";

        protected override string JsonPromptGemini15 => throw new NotImplementedException();

        protected override MessageRequest SetupPrompt(string jsonPromptPath)
        {
            var prompt = GetPromptFromJson.ConvertJson(jsonPromptPath);

            var prompts = new RequirementPromptBuilder(TestFiles, ExistingRequirement).BuildPrompt(prompt);

            return prompts;
        }
    }
}
