using LargeLanguageModelLibrary.Enums;
using LargeLanguageModelLibrary.LargeLanguageModels;
using LargeLanguageModelLibrary.Models;
namespace LargeLanguageModelLibrary
{
    public class LargeLanguageModel : ILargeLanguageModel
    {
        private readonly OpenAIClient openAIClient = new();
        private readonly AnthropicClient anthropicClient = new();

        public Task<MessageResponse> ChatAsync(ModelName modelName, MessageRequest messageRequest, bool debugMode = false, CancellationToken cancellationToken = default)
        {
            try
            {
                return modelName switch
                {
                    ModelName.GPT4o => openAIClient.CompleteChatAsync(messageRequest, debugMode: debugMode, cancellationToken: cancellationToken),
                    ModelName.Claude35Sonnet => anthropicClient.CompleteChatAsync(messageRequest, debugMode: debugMode, cancellationToken: cancellationToken),
                    _ => throw new ArgumentException("Invalid model name"),
                };
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
