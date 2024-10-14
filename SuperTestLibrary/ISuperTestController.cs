using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services;
using SuperTestLibrary.Services.Prompts;

namespace SuperTestLibrary
{
    public interface ISuperTestController
    {
        Task<SpecFlowFeatureFileResponse> GenerateSpecFlowFeatureFileAsync(string requirements);
        Task<IEnumerable<string>> GetAllReqIFFilesAsync();
        void SetLLM(ILargeLanguageModel llm);
        void SetGenerator(IGenerator generator);
    }
}
