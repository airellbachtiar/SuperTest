using SuperTestWPF.Models;

namespace SuperTestWPF.Services
{
    public interface IEvaluateFeatureFileService
    {
        Task<IEnumerable<PromptHistory>> EvaluateFeatureFileAsync(string selectedLlmString, SpecFlowFeatureFileModel featureFile, string requirements, CancellationToken cancellationToken);
        Task<IEnumerable<PromptHistory>> EvaluateSpecFlowScenarioAsync(string selectedLlmString, SpecFlowFeatureFileModel featureFile, string requirements, CancellationToken cancellationToken);
    }
}