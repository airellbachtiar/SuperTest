using SuperTestLibrary.Helpers;
using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services;
using SuperTestLibrary.Services.Prompts.ResponseModels;
using SuperTestLibrary.Storages;

namespace SuperTestLibrary
{
    public class SuperTestController : ISuperTestController
    {
        private readonly IReqIFStorage _reqIFStorage;

        public SuperTestController(IReqIFStorage reqIFStorage)
        {
            _reqIFStorage = reqIFStorage;
        }

        public async Task<SpecFlowFeatureFileResponse> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            CheckLLM();
            CheckGenerator();

            if (string.IsNullOrWhiteSpace(requirements))
            {
                throw new InvalidOperationException("No requirements provided.");
            }

            string response = await SelectedGenerator!.GenerateAsync(SelectedLLM!, requirements);

            var specFlowFeatureFile = GetSpecFlowFeatureFiles.ConvertJson(response);

            if (ValidateFeatureFile.Validate(specFlowFeatureFile))
            {
                return specFlowFeatureFile;
            }
            else
            {
                throw new InvalidOperationException("Unable to generate valid SpecFlow feature file.");
            }
        }

        public async Task<EvaluateSpecFlowFeatureFileResponse> EvaluateSpecFlowFeatureFileAsync(string featureFile)
        {
            CheckLLM();
            CheckGenerator();

            if (string.IsNullOrWhiteSpace(featureFile))
            {
                throw new InvalidOperationException("No feature file provided.");
            }

            string responseJson = await SelectedGenerator!.GenerateAsync(SelectedLLM!, featureFile);

            try
            {
                var response = GetSpecFlowFeatureFileEvaluation.ConvertJson(responseJson);
                return response;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Unable to evaluate SpecFlow feature file after 3 attempts.", e);
            }
        }

        public async Task<EvaluateSpecFlowScenarioResponse> EvaluateSpecFlowScenarioAsync(string featureFile)
        {
            CheckLLM();
            CheckGenerator();

            if (string.IsNullOrWhiteSpace(featureFile))
            {
                throw new InvalidOperationException("No feature file provided.");
            }

            string responseJson = await SelectedGenerator!.GenerateAsync(SelectedLLM!, featureFile);

            try
            {
                var response = GetSpecFlowScenarioEvaluation.ConvertJson(responseJson);
                return response;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Unable to evaluate SpecFlow scenario after 3 attempts.", e);
            }
        }

        public async Task<IEnumerable<string>> GetAllReqIFFilesAsync()
        {
            return await _reqIFStorage.GetAllReqIFsAsync();
        }

        private void CheckLLM()
        {
            if (SelectedLLM == null)
            {
                throw new InvalidOperationException("No LLM selected.");
            }
        }

        private void CheckGenerator()
        {
            if (SelectedGenerator == null)
            {
                throw new InvalidOperationException("No generator selected.");
            }
        }

        public IGenerator? SelectedGenerator { get; set; }
        public ILargeLanguageModel? SelectedLLM { get; set; }
    }

}
