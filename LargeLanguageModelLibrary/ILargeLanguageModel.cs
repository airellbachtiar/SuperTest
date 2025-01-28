using LargeLanguageModelLibrary.Enums;
using LargeLanguageModelLibrary.Models;

namespace LargeLanguageModelLibrary
{
    public interface ILargeLanguageModel
    {
        Task<MessageResponse> ChatAsync(ModelName modelName, MessageRequest messageRequest, bool debugMode = false, CancellationToken cancellationToken = default);
    }
}
