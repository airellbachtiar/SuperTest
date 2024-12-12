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

        public async Task EvaluateFeatureFileAsync(string selectedLlmString, SpecFlowFeatureFileModel featureFile, string requirements)
        {
            try
            {
                SetLlm(selectedLlmString);
                var evaluationResponse = await _retry.DoAsync(
                    () => _controller.EvaluateSpecFlowFeatureFileAsync(
                        requirements,
                        featureFile.FeatureFileContent),
                    TimeSpan.FromSeconds(1));
                AssignSpecFlowFeatureFileEvaluation.Assign(selectedLlmString, featureFile, evaluationResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception while evaluating {featureFile.FeatureFileName} using {selectedLlmString}");
            }
        }

        public async Task EvaluateSpecFlowScenarioAsync(string selectedLlmString, SpecFlowFeatureFileModel featureFile, string requirements)
        {
            try
            {
                SetLlm(selectedLlmString);
                var evaluationResponse = await _retry.DoAsync(
                    () => _controller.EvaluateSpecFlowScenarioAsync(
                        requirements,
                        featureFile.FeatureFileContent),
                    TimeSpan.FromSeconds(1));

                foreach (var scenario in evaluationResponse.ScenarioEvaluations)
                {
                    var scenarioModel = featureFile.Scenarios.First(s => s.Name == scenario.ScenarioName);
                    AssignScenarioEvaluation.Assign(selectedLlmString, scenarioModel, scenario);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception while evaluating {featureFile.FeatureFileName} using {selectedLlmString}");
            }
        }
    }
}
