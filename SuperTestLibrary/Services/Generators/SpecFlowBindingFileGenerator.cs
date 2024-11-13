using SuperTestLibrary.Helpers;
using SuperTestLibrary.Services.Prompts.Builders;

namespace SuperTestLibrary.Services.Generators
{
    public class SpecFlowBindingFileGenerator : GeneratorBase
    {
        public string? FeatureFile { get; set; }
        public Dictionary<string, string> GeneratedCSharpCode { get; set; } = [];

        protected override string _jsonPromptClaude_3_5_Sonnet => "Services/Prompts/SpecFlowBindingFile/Claude_3_5_Sonnet.json";
        protected override string _jsonPromptGPT_4o => "Services/Prompts/SpecFlowBindingFile/GPT_4o.json";
        protected override string _jsonPromptGemini_1_5 => throw new NotImplementedException("Gemini 1.5 is not supported for generating binding file.");

        protected override IEnumerable<string> SetupPrompt(string jsonPromptPath)
        {
            var prompt = GetPromptFromJson.ConvertJson(jsonPromptPath);

            var prompts = new SpecFlowBindingFilePromptBuilder(FeatureFile!, GeneratedCSharpCode).BuildPrompt(prompt);

            return prompts;
        }
    }
}
