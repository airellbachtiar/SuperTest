using SuperTestLibrary.Helpers;
using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services.Generators;
using SuperTestLibrary.Services.Prompts.ResponseModels;
using SuperTestLibrary.Storages;
using System.Text.Json;

namespace SuperTestLibrary
{
    public class SuperTestController : ISuperTestController
    {
        private readonly IReqIFStorage _reqIFStorage;
        private readonly SpecFlowFeatureFileGenerator specFlowFeatureFileGenerator = new();
        private readonly EvaluateSpecFlowFeatureFileGenerator evaluateSpecFlowFeatureFileGenerator = new();
        private readonly EvaluateSpecFlowScenarioGenerator evaluateSpecFlowScenarioGenerator = new();
        private readonly SpecFlowBindingFileGenerator specFlowBindingFileGenerator = new();

        public SuperTestController(IReqIFStorage reqIFStorage)
        {
            _reqIFStorage = reqIFStorage;
        }

        public async Task<SpecFlowFeatureFileResponse> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            ValidateInput(requirements, "requirements");
            specFlowFeatureFileGenerator.Requirements = requirements;
            SelectedGenerator = specFlowFeatureFileGenerator;
            string response = await GenerateAsync();
            var specFlowFeatureFile = GetSpecFlowFeatureFileResponse.ConvertJson(response);

            var gherkinDocuments = GetGherkinDocuments.ConvertSpecFlowFeatureFileResponse(specFlowFeatureFile);

            if (gherkinDocuments.Count != 0)
            {
                specFlowFeatureFile.GherkinDocuments = gherkinDocuments;
                return specFlowFeatureFile;
            }

            throw new InvalidOperationException("Unable to generate valid SpecFlow feature file.");
        }

        public async Task<EvaluateSpecFlowFeatureFileResponse> EvaluateSpecFlowFeatureFileAsync(string requirements, string featureFile)
        {
            ValidateInput(requirements, "requirements");
            ValidateInput(featureFile, "feature file");
            evaluateSpecFlowFeatureFileGenerator.FeatureFile = featureFile;
            evaluateSpecFlowFeatureFileGenerator.Requirements = requirements;

            SelectedGenerator = evaluateSpecFlowFeatureFileGenerator;
            return await EvaluateAsync<GetSpecFlowFeatureFileEvaluationResponse, EvaluateSpecFlowFeatureFileResponse>("Unable to evaluate SpecFlow feature file.");
        }

        public async Task<EvaluateSpecFlowScenarioResponse> EvaluateSpecFlowScenarioAsync(string requirements, string featureFile)
        {
            ValidateInput(requirements, "requirements");
            ValidateInput(featureFile, "feature file");
            evaluateSpecFlowScenarioGenerator.FeatureFile = featureFile;
            evaluateSpecFlowScenarioGenerator.Requirements = requirements;

            SelectedGenerator = evaluateSpecFlowScenarioGenerator;
            return await EvaluateAsync<GetSpecFlowScenarioEvaluationResponse, EvaluateSpecFlowScenarioResponse>("Unable to evaluate SpecFlow scenario.");
        }

        public async Task<SpecFlowBindingFileResponse> GenerateSpecFlowBindingFileAsync(string featureFile, Dictionary<string, string> generatedCSharpCode)
        {
            ValidateInput(featureFile, "feature file");
            specFlowBindingFileGenerator.FeatureFile = featureFile;
            specFlowBindingFileGenerator.GeneratedCSharpCode = generatedCSharpCode;

            SelectedGenerator = specFlowBindingFileGenerator;
            string response = await GenerateAsync();
            var specFlowBindingFile = GetSpecFlowBindingFileResponse.ConvertJson(response);

            // TODO: Validate binding file

            return specFlowBindingFile;
        }

        public async Task<IEnumerable<string>> GetAllReqIFFilesAsync()
        {
            return await _reqIFStorage.GetAllReqIFsAsync();
        }

        private async Task<string> GenerateAsync()
        {
            CheckLLM();
            CheckGenerator();
            return await SelectedGenerator!.GenerateAsync(SelectedLLM!);
        }

        private async Task<TResponse> EvaluateAsync<TConverter, TResponse>(string errorMessage)
        where TConverter : class
        {
            string responseJson = await GenerateAsync();

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

        private static void ValidateInput(string input, string inputName)
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

        public IGenerator? SelectedGenerator { get; private set; }
        public ILargeLanguageModel? SelectedLLM { get; set; }
    }

}
