using SuperTestWPF.Models;

namespace SuperTestWPF.Services
{
    public interface IFeatureFileGeneratorService
    {
        Task<SpecFlowFeatureFileResponse> GenerateSpecFlowFeatureFilesAsync(string selectedLlmString, string requirements);
    }
}