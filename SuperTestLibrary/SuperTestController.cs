using SuperTestLibrary.Helpers;
using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services.Generators;
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
            ValidateInput(requirements, "requirements");

            string response = await GenerateAsync(requirements);
            var specFlowFeatureFile = GetSpecFlowFeatureFileResponse.ConvertJson(response);

            if (ValidateFeatureFile.Validate(specFlowFeatureFile))
            {
                return specFlowFeatureFile;
            }

            throw new InvalidOperationException("Unable to generate valid SpecFlow feature file.");
        }

        public async Task<EvaluateSpecFlowFeatureFileResponse> EvaluateSpecFlowFeatureFileAsync(string featureFile)
        {
            ValidateInput(featureFile, "feature file");
            return await EvaluateAsync<GetSpecFlowFeatureFileEvaluationResponse, EvaluateSpecFlowFeatureFileResponse>(featureFile, "Unable to evaluate SpecFlow feature file after 3 attempts.");
        }

        public async Task<EvaluateSpecFlowScenarioResponse> EvaluateSpecFlowScenarioAsync(string featureFile)
        {
            ValidateInput(featureFile, "feature file");
            return await EvaluateAsync<GetSpecFlowScenarioEvaluationResponse, EvaluateSpecFlowScenarioResponse>(featureFile, "Unable to evaluate SpecFlow scenario after 3 attempts.");
        }

        public async Task<IEnumerable<string>> GetAllReqIFFilesAsync()
        {
            return await _reqIFStorage.GetAllReqIFsAsync();
        }

        private async Task<string> GenerateAsync(string input)
        {
            CheckLLM();
            CheckGenerator();
            return await SelectedGenerator!.GenerateAsync(SelectedLLM!, input);
        }

        private async Task<TResponse> EvaluateAsync<TConverter, TResponse>(string input, string errorMessage)
        where TConverter : class
        {
            string responseJson = await GenerateAsync(input);

            try
            {
                var response = (TResponse)typeof(TConverter).GetMethod("ConvertJson")!.Invoke(null, [responseJson])!;
                return response!;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(errorMessage, e);
            }
        }

        private void ValidateInput(string input, string inputName)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new InvalidOperationException($"No {inputName} provided.");
            }
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
