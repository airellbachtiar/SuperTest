using SuperTestLibrary.Models;
using LargeLanguageModelLibrary;
using LargeLanguageModelLibrary.Enums;
using LargeLanguageModelLibrary.Models;

namespace SuperTestLibrary.Services.Generators
{
    public abstract class GeneratorBase : IGenerator
    {
        public async Task<GeneratorResponse> GenerateAsync(ILargeLanguageModel largeLanguageModel, ModelName modelName, CancellationToken cancellationToken = default)
        {
            string jsonPromptPath = modelName switch
            {
                ModelName.Claude35Sonnet => JsonPromptClaude35Sonnet,
                ModelName.GPT4o => JsonPromptGPT4o,
                ModelName.Gemini15 => JsonPromptGemini15,
                _ => throw new InvalidOperationException("Unknown LLM."),
            };

            MessageRequest messageRequest = SetupPrompt(jsonPromptPath);

            messageRequest.Model = modelName switch
            {
                ModelName.Claude35Sonnet => "claude-3-5-sonnet-20241022",
                ModelName.GPT4o => "gpt-4o",
                _ => throw new InvalidOperationException("Unknown LLM."),
            };

            MessageResponse response = await largeLanguageModel.ChatAsync(modelName, messageRequest, true, cancellationToken);
            return new (response, messageRequest);
        }

        protected abstract MessageRequest SetupPrompt(string jsonPromptPath);

        protected abstract string JsonPromptClaude35Sonnet { get; }
        protected abstract string JsonPromptGPT4o { get; }
        protected abstract string JsonPromptGemini15 { get; }
    }
}
