using Microsoft.Extensions.Logging;
using SuperTestLibrary;
using SuperTestWPF.Retry;
using SuperTestWPF.Models;

namespace SuperTestWPF.Services
{
    public class RequirementGeneratorService(ISuperTestController controller, ILogger<RequirementGeneratorService> logger, IRetryService retry) : GeneratorBaseService(controller, logger), IRequirementGeneratorService
    {
        private readonly ISuperTestController _controller = controller;
        private readonly ILogger<RequirementGeneratorService> _logger = logger;
        private readonly IRetryService _retry = retry;

        public async Task<RequirementResponse> GenerateRequirementAsync(string selectedLlmString, Dictionary<string, string> testFiles, string existingRequirement, CancellationToken cancellationToken)
        {
            try
            {
                SetLlm(selectedLlmString);

                List<PromptHistory> promptHistories = [];

                _logger.LogInformation("Generating requirement...");
                var generatedRequirement = await _retry.DoAsync(
                    () => _controller.GenerateRequirementAsync(testFiles, existingRequirement, cancellationToken),
                    TimeSpan.FromSeconds(1));

                foreach (var prompt in generatedRequirement.Prompts)
                {
                    promptHistories.Add(new PromptHistory(DateTime.Now, "Generate Binding File", selectedLlmString, prompt));
                }

                return new(generatedRequirement.Requirement, promptHistories);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception while generating requirement using {_controller.SelectedLLM}");
                throw;
            }
        }
    }
}
