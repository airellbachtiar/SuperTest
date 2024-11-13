using SuperTestLibrary.Helpers;

namespace SuperTestLibrary.Services.Generators
{
    public class EvaluateSpecFlowScenarioGenerator : GeneratorBase
    {
        public string? Requirements { get; set; }
        public string? FeatureFile { get; set; }

        protected override string _jsonPromptClaude_3_5_Sonnet => "Services/Prompts/EvaluateSpecFlowScenario/Claude_3_5_Sonnet.json";
        protected override string _jsonPromptGPT_4o => "Services/Prompts/EvaluateSpecFlowScenario/GPT_4o.json";
        protected override string _jsonPromptGemini_1_5 => throw new NotImplementedException("Gemini 1.5 is not supported for evaluating SpecFlow scenario.");

        protected override IEnumerable<string> SetupPrompt(string jsonPromptPath)
        {
            return GetEvaluateSpecFlowPromptBuilder.SetupPrompt(jsonPromptPath, Requirements!, FeatureFile!);
        }
    }
}
