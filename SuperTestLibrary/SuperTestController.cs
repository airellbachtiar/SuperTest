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
        private ILargeLanguageModel? _llm;
        private IGenerator? _generator;
        private int _retryCounter = 0;

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

            string response = await _generator!.Generate(_llm!, requirements);

            SpecFlowFeatureFileResponse specFlowFeatureFile = GetSpecFlowFeatureFiles.ConvertJson(response);

            if (ValidateFeatureFile.Validate(specFlowFeatureFile))
            {
                _retryCounter = 0;
                return specFlowFeatureFile;
            }
            else
            {
                if (_retryCounter >= 3)
                {
                    _retryCounter = 0;
                    throw new InvalidOperationException("Unable to generate valid SpecFlow feature file after 3 attempts.");
                }

                _retryCounter++;
                return await GenerateSpecFlowFeatureFileAsync(requirements);
            }
            return await SelectedGenerator.Generate(SelectedLLM, requirements);
        }

        public async Task<EvaluateSpecFlowFeatureFileResponse> EvaluateSpecFlowFeatureFileAsync(string featureFile)
        {
            CheckLLM();
            CheckGenerator();

            if (string.IsNullOrWhiteSpace(featureFile))
            {
                throw new InvalidOperationException("No feature file provided.");
            }

            string response = await _generator!.Generate(_llm!, featureFile);

            return GetSpecFlowFeatureFileEvaluation.ConvertJson(response);
        }

        public async Task<IEnumerable<string>> GetAllReqIFFilesAsync()
        {
            return await _reqIFStorage.GetAllReqIFsAsync();
        }

        public IGenerator? SelectedGenerator { get; set; }
        public ILargeLanguageModel? SelectedLLM { get; set; }
    }

}
