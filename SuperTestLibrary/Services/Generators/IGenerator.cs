using LlmLibrary;
using SuperTestLibrary.Models;

namespace SuperTestLibrary.Services.Generators
{
    public interface IGenerator
    {
        Task<GeneratorResponse> GenerateAsync(ILargeLanguageModel largeLanguageModel, CancellationToken cancellationToken);
    }
}
