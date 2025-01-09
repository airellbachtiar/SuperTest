using Microsoft.Extensions.Logging;
using SuperTestLibrary;
using SuperTestWPF.Helper;
using SuperTestWPF.Models;
using SuperTestWPF.Retry;

namespace SuperTestWPF.Services
{
    public class EvaluateFeatureFileService : GeneratorBaseService, IEvaluateFeatureFileService
    {
        private readonly ISuperTestController _controller;
        private readonly ILogger<EvaluateFeatureFileService> _logger;
        private readonly IRetryService _retry;

        public EvaluateFeatureFileService(ISuperTestController controller, ILogger<EvaluateFeatureFileService> logger, IRetryService retry) : base(controller, logger)
        {
            _controller = controller;
            _logger = logger;
            _retry = retry;
        }

        public async Task<IEnumerable<PromptHistory>> EvaluateFeatureFileAsync(string selectedLlmString, SpecFlowFeatureFileModel featureFile, string requirements, CancellationToken cancellationToken = default)
        {
            try
            {
                SetLlm(selectedLlmString);
                var evaluationResponse = await _retry.DoAsync(
                    () => _controller.EvaluateSpecFlowFeatureFileAsync(
                        requirements,
                        featureFile.FeatureFileContent,
                        cancellationToken),
                    TimeSpan.FromSeconds(1));
                AssignSpecFlowFeatureFileEvaluation.Assign(selectedLlmString, featureFile, evaluationResponse);

                var promptHistory = evaluationResponse.Prompts
                    .Select(prompt => new PromptHistory(DateTime.Now, "Evaluate Feature File", selectedLlmString, prompt)).ToList();

                for (int i = 0; i < evaluationResponse.RawResponse.Count && i < evaluationResponse.RawResponse.Count; i++)
                {
                    promptHistory[i].RawResponse = evaluationResponse.RawResponse[i];
                }

                return promptHistory;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception while evaluating {featureFile.FeatureFileName} using {selectedLlmString}");
                throw;
            }
        }

        public async Task<IEnumerable<PromptHistory>> EvaluateSpecFlowScenarioAsync(string selectedLlmString, SpecFlowFeatureFileModel featureFile, string requirements, CancellationToken cancellationToken = default)
        {
            try
            {
                SetLlm(selectedLlmString);
                var evaluationResponse = await _retry.DoAsync(
                    () => _controller.EvaluateSpecFlowScenarioAsync(
                        requirements,
                        featureFile.FeatureFileContent,
                        cancellationToken),
                    TimeSpan.FromSeconds(1));

                foreach (var scenario in evaluationResponse.ScenarioEvaluations)
                {
                    var scenarioModel = featureFile.Scenarios.First(s => s.Name == scenario.ScenarioName);
                    AssignScenarioEvaluation.Assign(selectedLlmString, scenarioModel, scenario);
                }

                var promptHistory = evaluationResponse.Prompts
                    .Select(prompt => new PromptHistory(DateTime.Now, "Evaluate Feature File", selectedLlmString, prompt)).ToList();

                for (int i = 0; i < evaluationResponse.RawResponse.Count && i < evaluationResponse.RawResponse.Count; i++)
                {
                    promptHistory[i].RawResponse = evaluationResponse.RawResponse[i];
                }

                return promptHistory;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception while evaluating {featureFile.FeatureFileName} using {selectedLlmString}");
                throw;
            }
        }
    }
}
