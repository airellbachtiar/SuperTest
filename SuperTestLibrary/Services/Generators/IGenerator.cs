using LlmLibrary;

namespace SuperTestLibrary.Services.Generators
{
    public interface IGenerator
    {
        Task<string> GenerateAsync(ILargeLanguageModel largeLanguageModel);
    }
}
