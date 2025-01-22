using LargeLanguageModelLibrary.Models;

namespace LargeLanguageModelLibrary
{
    public interface ILargeLanguageModel
    {
        Task<MessageResponse> ChatAsync(MessageRequest messageRequest, CancellationToken cancellationToken);
    }
}
