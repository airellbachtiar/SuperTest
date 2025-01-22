using LargeLanguageModelLibrary.Enums;
using LargeLanguageModelLibrary.Models;
namespace LargeLanguageModelLibrary
{
    public class LargeLanguageModel
    {
        public Task<MessageResponse> ChatAsync(ModelName modelName, MessageRequest messageRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                switch (modelName)
                {
                    case ModelName.GPT4o:
                        throw new NotImplementedException();
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
