using LlmLibrary;

namespace SuperTestLibrary.Services.Generators
{
    public abstract class GeneratorBase : IGenerator
    {
        public async Task<string> GenerateAsync(ILargeLanguageModel largeLanguageModel)
        {
            string jsonPromptPath = largeLanguageModel.Id switch
            {
                "Claude 3.5 Sonnet" => JsonPromptClaude35Sonnet,
                "GPT-4o" => JsonPromptGPT4o,
                "Gemini 1.5" => JsonPromptGemini15,
                _ => throw new InvalidOperationException("Unknown LLM."),
            };

            IEnumerable<string> prompts = SetupPrompt(jsonPromptPath);
            string response = await largeLanguageModel.CallAsync(prompts);
            return response;
        }

        protected abstract IEnumerable<string> SetupPrompt(string jsonPromptPath);

        protected abstract string JsonPromptClaude35Sonnet { get; }
        protected abstract string JsonPromptGPT4o { get; }
        protected abstract string JsonPromptGemini15 { get; }
    }
}
