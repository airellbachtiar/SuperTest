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
            get
            {
                if (GeneratedCSharpCode.Count == 0)
                {
                    return "Prompts/GenerateSpecFlowBindingFile/Claude_3_5_Sonnet_NoCode.json";
                }
                return "Prompts/GenerateSpecFlowBindingFile/Claude_3_5_Sonnet.json";
            }
        }
        protected override string JsonPromptGPT4o
        {
            get
            {
                if (GeneratedCSharpCode.Count == 0)
                {
                    return "Prompts/GenerateSpecFlowBindingFile/GPT_4o_NoCode.json";
                }
                return "Prompts/GenerateSpecFlowBindingFile/GPT_4o.json";
            }
        }
        protected override string JsonPromptGemini15 => throw new NotImplementedException("Gemini 1.5 is not supported for generating binding file.");

        protected override IEnumerable<string> SetupPrompt(string jsonPromptPath)
        {
            var prompt = GetPromptFromJson.ConvertJson(jsonPromptPath);

            var prompts = new SpecFlowBindingFilePromptBuilder(FeatureFile!, GeneratedCSharpCode).BuildPrompt(prompt);

            return prompts;
        }
    }
}
