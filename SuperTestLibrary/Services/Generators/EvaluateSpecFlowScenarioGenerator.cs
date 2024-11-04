using SuperTestLibrary.LLMs;

namespace SuperTestLibrary.Services.Generators
{
    public class EvaluateSpecFlowScenarioGenerator : EvaluateSpecFlowFile, IGenerator
    {
        private readonly string _requirements;

        private const string _jsonPromptClaude_3_5_Sonnet = "Services/Prompts/EvaluateSpecFlowScenario/Claude_3_5_Sonnet.json";
        private const string _jsonPromptGPT_4o = "Services/Prompts/EvaluateSpecFlowScenario/GPT_4o.json";

        public EvaluateSpecFlowScenarioGenerator(string requirements)
        {
            _requirements = requirements;
        }

        public async Task<string> GenerateAsync(ILargeLanguageModel largeLanguageModel, string featureFile)
        {
            string jsonPromptPath = string.Empty;
            if (!string.IsNullOrEmpty(_requirements))
            {
                jsonPromptPath = largeLanguageModel.Id switch
                {
                    "Claude 3.5 Sonnet" => _jsonPromptClaude_3_5_Sonnet,
                    "GPT-4o" => _jsonPromptGPT_4o,
                    _ => throw new InvalidOperationException("Unknown LLM."),
                };
            }
            else
            {
                throw new InvalidOperationException("No requirements provided.");
            }

            IEnumerable<string> prompts = SetupPrompt(jsonPromptPath, _requirements, featureFile);

            return await largeLanguageModel.Call(prompts);
        }
    }
}
