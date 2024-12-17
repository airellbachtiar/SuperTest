using LlmLibrary;
using SuperTestLibrary.Services.Generators;
using SuperTestLibrary.Services.PromptBuilders.ResponseModels;
using SuperTestLibrary.Storages;

namespace SuperTestLibrary
{
    public interface ISuperTestController
    {
        IGenerator? SelectedGenerator { get; }
        ILargeLanguageModel? SelectedLLM { get; set; }
        Task<SpecFlowFeatureFileResponse> GenerateSpecFlowFeatureFileAsync(string requirements, CancellationToken cancellationToken);
        Task<EvaluateSpecFlowFeatureFileResponse> EvaluateSpecFlowFeatureFileAsync(string requirements, string featureFile, CancellationToken cancellationToken);
        Task<EvaluateSpecFlowScenarioResponse> EvaluateSpecFlowScenarioAsync(string requirements, string featureFile, CancellationToken cancellationToken);
        Task<SpecFlowBindingFileResponse> GenerateSpecFlowBindingFileAsync(string featureFile, Dictionary<string, string> generatedCSharpCode, CancellationToken cancellationToken);
        Task<RequirementResponse> GenerateRequirementAsync(Dictionary<string, string> testFiles, string existingRequirement, CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetAllReqIFFilesAsync();
        public string GetStorageLocation();
        public void UpdateStorageLocation(string newPath);
    }
}
