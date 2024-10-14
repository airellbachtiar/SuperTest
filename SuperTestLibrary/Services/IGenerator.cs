using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services.Prompts;

namespace SuperTestLibrary.Services
{
    public interface IGenerator
    {
        Task<SpecFlowFeatureFileResponse> Generate(ILargeLanguageModel largeLanguageModel, string additionalMessage);
    }
}
