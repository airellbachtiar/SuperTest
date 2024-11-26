using SuperTestWPF.Models;

namespace SuperTestWPF.Services
{
    public interface IEvaluateFeatureFileService
    {
        Task EvaluateFeatureFileAsync(string selectedLlmString, SpecFlowFeatureFileModel featureFile, string requirements);
        Task EvaluateSpecFlowScenarioAsync(string selectedLlmString, SpecFlowFeatureFileModel featureFile, string requirements);
    }
}