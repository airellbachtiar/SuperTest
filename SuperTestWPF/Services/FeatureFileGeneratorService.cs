using Microsoft.Extensions.Logging;
using SuperTestLibrary;
using SuperTestWPF.Helper;
using SuperTestWPF.Models;
using SuperTestWPF.Retry;
namespace SuperTestWPF.Services
{
    public class FeatureFileGeneratorService : GeneratorBaseService, IFeatureFileGeneratorService
    {
        private readonly ILogger<FeatureFileGeneratorService> _logger;
        private readonly ISuperTestController _controller;
        private readonly IRetryService _retry;

        public FeatureFileGeneratorService(ISuperTestController controller, ILogger<FeatureFileGeneratorService> logger, IRetryService retry) : base(controller, logger)
        {
            _controller = controller;
            _logger = logger;
            _retry = retry;
        }

        public async Task<SpecFlowFeatureFileResponse> GenerateSpecFlowFeatureFilesAsync(string selectedLlmString, string requirements, CancellationToken cancellationToken = default)
        {
            try
            {
                SetLlm(selectedLlmString);

                List<SpecFlowFeatureFileModel> specFlowFeatureFileModels = [];
                List<PromptHistory> promptHistories = [];

                var featureFileResponse = await _retry.DoAsync(
                    () => _controller.GenerateSpecFlowFeatureFileAsync(requirements, cancellationToken),
                    TimeSpan.FromSeconds(1));

                for (int i = 0; i < featureFileResponse.FeatureFiles.Count; i++)
                {
                    var featureFileModel = featureFileResponse.FeatureFiles.ElementAt(i);
                    var gherkinDocument = featureFileResponse.GherkinDocuments[i];

                    if (gherkinDocument == null)
                    {
                        continue;
                    }

                    var featureFile = GetSpecFlowFeatureFileModel.ConvertSpecFlowFeatureFileResponse(featureFileModel, gherkinDocument);
                    specFlowFeatureFileModels.Add(featureFile);
                }

                foreach (var prompt in featureFileResponse.Prompts)
                {
                    promptHistories.Add(new PromptHistory(DateTime.Now, "Generate Feature File", selectedLlmString, prompt));
                }

                for (int i = 0; i < featureFileResponse.RawResponse.Count && i < promptHistories.Count; i++)
                {
                    promptHistories[i].RawResponse = featureFileResponse.RawResponse[i];
                }

                if (specFlowFeatureFileModels.Count == 0)
                {
                    _logger.LogWarning("Feature file is empty. Failed to generate feature file.");
                }
                else _logger.LogInformation("Feature file generated.");

                return new(specFlowFeatureFileModels, promptHistories);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while generating SpecFlow feature file.");
                throw;
            }
        }
    }
}
