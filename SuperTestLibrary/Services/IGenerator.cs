using SuperTestLibrary.LLMs;

namespace SuperTestLibrary.Services
{
    public interface IGenerator
    {
        Task<string> Generate(ILargeLanguageModel largeLanguageModel, string additionalMessage);
    }
}
