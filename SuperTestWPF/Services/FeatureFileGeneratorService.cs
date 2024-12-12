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

        public async Task<IEnumerable<SpecFlowFeatureFileModel>> GenerateSpecFlowFeatureFilesAsync(string selectedLlmString, string requirements)
        {
            try
            {
                SetLlm(selectedLlmString);

                List<SpecFlowFeatureFileModel> specFlowFeatureFileModels = [];

                var featureFileResponse = await _retry.DoAsync(
                    () => _controller.GenerateSpecFlowFeatureFileAsync(requirements),
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

                if (!specFlowFeatureFileModels.Any())
                {
                    _logger.LogWarning("Feature file is empty. Failed to generate feature file.");
                }
                else _logger.LogInformation("Feature file generated.");

                return specFlowFeatureFileModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while generating SpecFlow feature file.");
                throw;
            }
        }
    }
}
