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
                    promptHistories.Add(new PromptHistory(DateTime.Now, "Generate Requirement", selectedLlmString, prompt));
                }

                for (int i = 0; i < generatedRequirement.RawResponse.Count && i < promptHistories.Count; i++)
                {
                    promptHistories[i].RawResponse = generatedRequirement.RawResponse[i];
                }

                RequirementResponse requirementResponse = new(generatedRequirement.Response,
                    generatedRequirement.Requirements.Select(r => new RequirementModel
                    {
                        Id = r.Id,
                        Content = r.Content,
                        Trace = r.Trace,
                    }),
                    promptHistories);

                requirementResponse.Title = generatedRequirement.Title;
                requirementResponse.FileName = generatedRequirement.FileName;
                return requirementResponse;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation was cancelled.");
                throw;
            }
            catch (Exception)
            {
                _logger.LogError($"Exception while generating requirement using {_controller.SelectedLLM!.Id}");
                throw;
            }
        }
    }
}
