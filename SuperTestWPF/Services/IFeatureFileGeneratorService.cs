using SuperTestWPF.Models;

namespace SuperTestWPF.Services
{
    public interface IFeatureFileGeneratorService
    {
        Task<IEnumerable<SpecFlowFeatureFileModel>> GenerateSpecFlowFeatureFilesAsync(string selectedLlmString, string requirements);
    }
}