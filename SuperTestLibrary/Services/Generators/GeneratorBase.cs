using SuperTestLibrary.LLMs;

namespace SuperTestLibrary.Services.Generators
{
    public abstract class GeneratorBase : IGenerator
    {
        public async Task<string> GenerateAsync(ILargeLanguageModel largeLanguageModel)
        {
            string jsonPromptPath = largeLanguageModel.Id switch
            {
                "Claude 3.5 Sonnet" => _jsonPromptClaude_3_5_Sonnet,
                "GPT-4o" => _jsonPromptGPT_4o,
                "Gemini 1.5" => _jsonPromptGemini_1_5,
                _ => throw new InvalidOperationException("Unknown LLM."),
            };

            IEnumerable<string> prompts = SetupPrompt(jsonPromptPath);
            string response = await largeLanguageModel.CallAsync(prompts);
            return response;
        }

        protected abstract IEnumerable<string> SetupPrompt(string jsonPromptPath);

        protected abstract string _jsonPromptClaude_3_5_Sonnet { get; }
        protected abstract string _jsonPromptGPT_4o { get; }
        protected abstract string _jsonPromptGemini_1_5 { get; }
    }
}
