using LargeLanguageModelLibrary;
using LargeLanguageModelLibrary.Enums;
using SuperTestLibrary.Models;

namespace SuperTestLibrary.Services.Generators
{
    public interface IGenerator
    {
        Task<GeneratorResponse> GenerateAsync(ILargeLanguageModel largeLanguageModel, ModelName modelName, CancellationToken cancellationToken);
    }
}
