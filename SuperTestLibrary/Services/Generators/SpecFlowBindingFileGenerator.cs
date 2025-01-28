using LargeLanguageModelLibrary.Models;
using SuperTestLibrary.Helpers;
using SuperTestLibrary.Services.PromptBuilders;

namespace SuperTestLibrary.Services.Generators
{
    public class SpecFlowBindingFileGenerator : GeneratorBase
    {
        public string? FeatureFile { get; set; }
        public Dictionary<string, string> GeneratedCSharpCode { get; set; } = [];

        protected override string JsonPromptClaude35Sonnet
        {
            get { return GetPromptPath(); }
        }
        protected override string JsonPromptGPT4o
        {
            get { return GetPromptPath(); }
        }
        protected override string JsonPromptGemini15 => throw new NotImplementedException("Gemini 1.5 is not supported for generating binding file.");

        protected override MessageRequest SetupPrompt(string jsonPromptPath)
        {
            var prompt = GetPromptFromJson.ConvertJson(jsonPromptPath);

            var prompts = new SpecFlowBindingFilePromptBuilder(FeatureFile!, GeneratedCSharpCode).BuildPrompt(prompt);

            return prompts;
        }

        private string GetPromptPath()
        {
            if (GeneratedCSharpCode.Count == 0)
            {
                return "Prompts/GenerateSpecFlowBindingFile/BindingFilePromptNoCode.json";
            }
            return "Prompts/GenerateSpecFlowBindingFile/BindingFilePrompt.json";
        }
    }
}
