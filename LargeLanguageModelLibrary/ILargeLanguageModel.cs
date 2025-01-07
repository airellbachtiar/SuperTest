using LargeLanguageModelLibrary.Models;

namespace LargeLanguageModelLibrary
{
    public interface ILargeLanguageModel
    {
        string ModelName { get; }
        Task<MessageResponse> ChatAsync(MessageRequest messageRequest, CancellationToken cancellationToken);
    }
}
