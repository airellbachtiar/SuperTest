using SuperTestWPF.Models;

namespace SuperTestWPF.Services
{
    public interface IRequirementGeneratorService
    {
        Task<RequirementResponse> GenerateRequirementAsync(string selectedLlmString, Dictionary<string, string> testFiles, string existingRequirement, CancellationToken cancellationToken);
    }
}