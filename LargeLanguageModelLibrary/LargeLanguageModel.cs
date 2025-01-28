using LargeLanguageModelLibrary.Enums;
using LargeLanguageModelLibrary.LargeLanguageModels;
using LargeLanguageModelLibrary.Models;
namespace LargeLanguageModelLibrary
{
    public class LargeLanguageModel : ILargeLanguageModel
    {
        private readonly OpenAIClient openAIClient = new();

        public Task<MessageResponse> ChatAsync(ModelName modelName, MessageRequest messageRequest, bool debugMode = false, CancellationToken cancellationToken = default)
        {
            try
            {
                switch (modelName)
                {
                    case ModelName.GPT4o:
                        return openAIClient.CompleteChatAsync(messageRequest, debugMode: debugMode, cancellationToken: cancellationToken);
                    case ModelName.Claude35Sonnet:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentException("Invalid model name");
                }
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
