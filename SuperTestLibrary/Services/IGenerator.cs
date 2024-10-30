using SuperTestLibrary.LLMs;

namespace SuperTestLibrary.Services
{
    public interface IGenerator
    {
        Task<string> GenerateAsync(ILargeLanguageModel largeLanguageModel, string additionalMessage);
    }
}
