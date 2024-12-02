using LlmLibrary;
using Microsoft.Extensions.Logging;
using SuperTestLibrary.Helpers;
using SuperTestLibrary.Services.Generators;
using SuperTestLibrary.Services.PromptBuilders.ResponseModels;
using SuperTestLibrary.Storages;

namespace SuperTestLibrary
{
    public class SuperTestController : ISuperTestController
    {
        private readonly IReqIFStorage _reqIFStorage;
        private readonly SpecFlowFeatureFileGenerator specFlowFeatureFileGenerator = new();
        private readonly EvaluateSpecFlowFeatureFileGenerator evaluateSpecFlowFeatureFileGenerator = new();
        private readonly EvaluateSpecFlowScenarioGenerator evaluateSpecFlowScenarioGenerator = new();
        private readonly SpecFlowBindingFileGenerator specFlowBindingFileGenerator = new();

        private readonly ILogger<SuperTestController> _logger;

        public SuperTestController(IReqIFStorage reqIFStorage, ILogger<SuperTestController> logger)
        {
            _reqIFStorage = reqIFStorage;
            _logger = logger;
        }

        public async Task<SpecFlowFeatureFileResponse> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            _logger.LogInformation("Starting {MethodName}.", nameof(GenerateSpecFlowFeatureFileAsync));
            ValidateInput(requirements, "requirements");

            specFlowFeatureFileGenerator.Requirements = requirements;
            SelectedGenerator = specFlowFeatureFileGenerator;
            _logger.LogInformation("Generator {GeneratorName} selected for SpecFlow feature file generation.", nameof(SpecFlowFeatureFileGenerator));

            string response = await GenerateAsync();

            _logger.LogInformation("{LLMId} generated a response successfully. Processing the response.", SelectedLLM?.Id ?? "Unknown LLM");
            _logger.LogInformation("Converting the response JSON to SpecFlow feature file format.");
            var specFlowFeatureFile = GetSpecFlowFeatureFileResponse.ConvertJson(response);
            _logger.LogInformation("SpecFlow feature file conversion successful.");

            _logger.LogInformation("Validating Gherkin documents from the SpecFlow feature file.");
            var gherkinDocuments = GetGherkinDocuments.ConvertSpecFlowFeatureFileResponse(specFlowFeatureFile);

            if (gherkinDocuments.Count != 0)
            {
                specFlowFeatureFile.GherkinDocuments = gherkinDocuments;
                _logger.LogInformation("SpecFlow feature file validation succeeded. Feature file generation complete.");
                return specFlowFeatureFile;
            }

            _logger.LogError("SpecFlow feature file validation failed. No valid Gherkin documents found.");
            throw new InvalidOperationException("Unable to generate valid SpecFlow feature file.");
        }

        public async Task<EvaluateSpecFlowFeatureFileResponse> EvaluateSpecFlowFeatureFileAsync(string requirements, string featureFile)
        {
            _logger.LogInformation("Starting {MethodName}.", nameof(EvaluateSpecFlowFeatureFileAsync));
            ValidateInput(requirements, "requirements");
            ValidateInput(featureFile, "feature file");

            evaluateSpecFlowFeatureFileGenerator.FeatureFile = featureFile;
            evaluateSpecFlowFeatureFileGenerator.Requirements = requirements;
            SelectedGenerator = evaluateSpecFlowFeatureFileGenerator;

            _logger.LogInformation("Generator {GeneratorName} selected for SpecFlow feature file evaluation.", nameof(EvaluateSpecFlowFeatureFileGenerator));
            return await EvaluateAsync<GetSpecFlowFeatureFileEvaluationResponse, EvaluateSpecFlowFeatureFileResponse>("SpecFlow feature file evaluation failed.");
        }

        public async Task<EvaluateSpecFlowScenarioResponse> EvaluateSpecFlowScenarioAsync(string requirements, string featureFile)
        {
            _logger.LogInformation("Starting {MethodName}.", nameof(EvaluateSpecFlowScenarioAsync));
            ValidateInput(requirements, "requirements");
            ValidateInput(featureFile, "feature file");

            evaluateSpecFlowScenarioGenerator.FeatureFile = featureFile;
            evaluateSpecFlowScenarioGenerator.Requirements = requirements;
            SelectedGenerator = evaluateSpecFlowScenarioGenerator;

            _logger.LogInformation("Generator {GeneratorName} selected for SpecFlow scenario evaluation.", nameof(EvaluateSpecFlowScenarioGenerator));
            return await EvaluateAsync<GetSpecFlowScenarioEvaluationResponse, EvaluateSpecFlowScenarioResponse>("SpecFlow scenario evaluation failed.");
        }

        public async Task<SpecFlowBindingFileResponse> GenerateSpecFlowBindingFileAsync(string featureFile, Dictionary<string, string> generatedCSharpCode)
        {
            _logger.LogInformation("Starting {MethodName}.", nameof(GenerateSpecFlowBindingFileAsync));

            ValidateInput(featureFile, "feature file");
            _logger.LogInformation("Feature file validated successfully.");

            if (generatedCSharpCode == null || !generatedCSharpCode.Any())
            {
                _logger.LogError("Generated C# code is null or empty.");
                throw new InvalidOperationException("Generated C# code cannot be null or empty.");
            }
            _logger.LogInformation("Generated C# code validated successfully.");

            specFlowBindingFileGenerator.FeatureFile = featureFile;
            specFlowBindingFileGenerator.GeneratedCSharpCode = generatedCSharpCode;
            SelectedGenerator = specFlowBindingFileGenerator;

            string response = await GenerateAsync();
            _logger.LogInformation("Response successfully generated by {LLMName}.", SelectedLLM?.Id ?? "Unknown LLM");

            _logger.LogInformation("Converting response JSON to SpecFlow binding file object.");
            var specFlowBindingFile = GetSpecFlowBindingFileResponse.ConvertJson(response);
            _logger.LogInformation("SpecFlow binding file conversion completed successfully.");

            _logger.LogInformation("{MethodName} completed successfully.", nameof(GenerateSpecFlowBindingFileAsync));
            return specFlowBindingFile;
        }

        public async Task<IEnumerable<string>> GetAllReqIFFilesAsync()
        {
            _logger.LogInformation("Getting all ReqIF files.");
            var files = await _reqIFStorage.GetAllReqIFsAsync();
            _logger.LogInformation("Successfully fetched all ReqIF files.");

            return files;
        }

        private async Task<string> GenerateAsync()
        {
            _logger.LogInformation("Starting response generation using {GeneratorName} with {LLMName}.", SelectedGenerator?.GetType().Name ?? "Unknown Generator", SelectedLLM?.Id ?? "Unknown LLM");
            CheckLLM();
            CheckGenerator();

            string response = await SelectedGenerator!.GenerateAsync(SelectedLLM!);
            _logger.LogInformation("Response generation completed successfully.");
            return response;
        }

        private async Task<TResponse> EvaluateAsync<TConverter, TResponse>(string errorMessage)
        where TConverter : class
        {
            _logger.LogInformation("Starting evaluation process.");
            string responseJson = await GenerateAsync();
            _logger.LogInformation("Response JSON successfully generated.");

            try
            {
                _logger.LogInformation("Converting response JSON to {ResponseType}.", typeof(TResponse).Name);
                var response = (TResponse)typeof(TConverter).GetMethod("ConvertJson")!.Invoke(null, new object[] { responseJson })!;
                _logger.LogInformation("Response JSON successfully converted to {ResponseType}.", typeof(TResponse).Name);
                return response!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                throw new InvalidOperationException(errorMessage, ex);
            }
        }

        private void ValidateInput(string input, string inputName)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                _logger.LogError($"No {inputName} provided.");
                throw new InvalidOperationException($"No {inputName} provided.");
            }
        }

        private void CheckLLM()
        {
            if (SelectedLLM == null)
            {
                _logger.LogError("No LLM selected.");
                throw new InvalidOperationException("No LLM selected.");
            }
        }

        private void CheckGenerator()
        {
            if (SelectedGenerator == null)
            {
                _logger.LogError("No generator selected.");
                throw new InvalidOperationException("No generator selected.");
            }
        }

        public IGenerator? SelectedGenerator { get; private set; }
        public ILargeLanguageModel? SelectedLLM { get; set; }
    }

}
