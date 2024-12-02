using SuperTestLibrary.Helpers;

namespace SuperTestLibrary.Services.Generators
{
    public class EvaluateSpecFlowFeatureFileGenerator : GeneratorBase
    {
        public string? FeatureFile { get; set; }
        public string? Requirements { get; set; }

        protected override string JsonPromptClaude35Sonnet => "Prompts/EvaluateSpecFlowFeatureFile/Claude_3_5_Sonnet.json";
        protected override string JsonPromptGPT4o => "Prompts/EvaluateSpecFlowFeatureFile/GPT_4o.json";
        protected override string JsonPromptGemini15 => throw new NotImplementedException("Gemini 1.5 is not supported for evaluating SpecFlow feature file.");

        protected override IEnumerable<string> SetupPrompt(string jsonPromptPath)
        {
            return GetEvaluateSpecFlowPromptBuilder.SetupPrompt(jsonPromptPath, Requirements!, FeatureFile!);
        }
    }
}
