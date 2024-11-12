using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services.Generators;
using SuperTestLibrary.Services.Prompts.ResponseModels;

namespace SuperTestLibrary
{
    public interface ISuperTestController
    {
        IGenerator? SelectedGenerator { get; }
        ILargeLanguageModel? SelectedLLM { get; set; }
        Task<SpecFlowFeatureFileResponse> GenerateSpecFlowFeatureFileAsync(string requirements);
        Task<EvaluateSpecFlowFeatureFileResponse> EvaluateSpecFlowFeatureFileAsync(string requirements, string featureFile);
        Task<EvaluateSpecFlowScenarioResponse> EvaluateSpecFlowScenarioAsync(string requirements, string featureFile);
        Task<SpecFlowBindingFileResponse> GenerateSpecFlowBindingFileAsync(string featureFile, Dictionary<string, string> generatedCSharpCode);
        Task<IEnumerable<string>> GetAllReqIFFilesAsync();
    }
}
