using SuperTestLibrary.Helpers;
using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services.Prompts.Builders;

namespace SuperTestLibrary.Services.Generators
{
    public class SpecFlowBindingFileGenerator : GeneratorBase
    {
        private readonly string _featureFile;
        private readonly Dictionary<string, string> _generatedCSharpCode;

        protected override string _jsonPromptClaude_3_5_Sonnet => "Services/Prompts/SpecFlowBindingFile/Claude_3_5_Sonnet.json";
        protected override string _jsonPromptGPT_4o => "Services/Prompts/SpecFlowBindingFile/GPT_4o.json";

        public SpecFlowBindingFileGenerator(string featureFile, Dictionary<string, string> generatedCSharpCode)
        {
            _generatedCSharpCode = generatedCSharpCode;
            _featureFile = featureFile;
        }

        protected override IEnumerable<string> SetupPrompt(string jsonPromptPath)
        {
            var prompt = GetPromptFromJson.ConvertJson(jsonPromptPath);

            var prompts = new SpecFlowBindingFilePromptBuilder(_featureFile, _generatedCSharpCode).BuildPrompt(prompt);

            return prompts;
        }
    }
}
