using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services.Generators;
using SuperTestLibrary.Services.Prompts.ResponseModels;

namespace SuperTestLibrary
{
    public interface ISuperTestController
    {
        IGenerator? SelectedGenerator { get; set; }
        ILargeLanguageModel? SelectedLLM { get; set; }
        Task<SpecFlowFeatureFileResponse> GenerateSpecFlowFeatureFileAsync(string requirements);
        Task<EvaluateSpecFlowFeatureFileResponse> EvaluateSpecFlowFeatureFileAsync(string featureFile);
        Task<EvaluateSpecFlowScenarioResponse> EvaluateSpecFlowScenarioAsync(string featureFile);
        Task<IEnumerable<string>> GetAllReqIFFilesAsync();
    }
}
