using LargeLanguageModelLibrary;
using LargeLanguageModelLibrary.Enums;
using Microsoft.Extensions.Logging;
using SuperTestLibrary.Helpers;
using SuperTestLibrary.Models;
using SuperTestLibrary.Services.Generators;
using SuperTestLibrary.Storages;
using System.Text.RegularExpressions;

namespace SuperTestLibrary
{
    public class SuperTestController : ISuperTestController
    {
        private readonly IReqIFStorage _reqIFStorage;
        private readonly ILargeLanguageModel _largeLanguageModel;
        private readonly SpecFlowFeatureFileGenerator specFlowFeatureFileGenerator = new();
        private readonly EvaluateSpecFlowFeatureFileGenerator evaluateSpecFlowFeatureFileGenerator = new();
        private readonly EvaluateSpecFlowScenarioGenerator evaluateSpecFlowScenarioGenerator = new();
        private readonly SpecFlowBindingFileGenerator specFlowBindingFileGenerator = new();
        private readonly RequirementGenerator requirementGenerator = new();

        private readonly ILogger<SuperTestController> _logger;

        public IGenerator? SelectedGenerator { get; private set; }
        public ModelName? SelectedLLM { get; set; }

        public SuperTestController(IReqIFStorage reqIFStorage, ILargeLanguageModel largeLanguageModel, ILogger<SuperTestController> logger)
        {
            _reqIFStorage = reqIFStorage;
            _largeLanguageModel = largeLanguageModel;
            _logger = logger;
        }

        public async Task<SpecFlowFeatureFileResponse> GenerateSpecFlowFeatureFileAsync(string requirements, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting {MethodName}.", nameof(GenerateSpecFlowFeatureFileAsync));
            ValidateInput(requirements, "requirements");

            specFlowFeatureFileGenerator.Requirements = requirements;
            SelectedGenerator = specFlowFeatureFileGenerator;
            _logger.LogInformation("Generator {GeneratorName} selected for SpecFlow feature file generation.", nameof(SpecFlowFeatureFileGenerator));

            var response = await GenerateAsync(cancellationToken);

            _logger.LogInformation("{LLMId} generated a response successfully. Processing the response.", SelectedLLM.ToString() ?? "Unknown LLM");
            _logger.LogInformation("Converting the response JSON to SpecFlow feature file format.");
            var specFlowFeatureFile = GetSpecFlowFeatureFileResponse.ConvertJson(response.Response.Messages.Last().Text);
            specFlowFeatureFile.RawResponse.AddRange(response.Response.Messages.Select(m => m.Text));
            _logger.LogInformation("SpecFlow feature file conversion successful.");

            UnescapeDictionary(specFlowFeatureFile.FeatureFiles);

            _logger.LogInformation("Validating Gherkin documents from the SpecFlow feature file.");
            var gherkinDocuments = GetGherkinDocuments.ConvertSpecFlowFeatureFileResponse(specFlowFeatureFile);

            if (gherkinDocuments.Count != 0)
            {
                specFlowFeatureFile.GherkinDocuments = gherkinDocuments;
                _logger.LogInformation("SpecFlow feature file validation succeeded. Feature file generation complete.");

                specFlowFeatureFile.Prompts = response.Request.Messages.Select(m => m.Content.Text).ToList();

                return specFlowFeatureFile;
            }

            _logger.LogError("SpecFlow feature file validation failed. No valid Gherkin documents found.");
            throw new InvalidOperationException("Unable to generate valid SpecFlow feature file.");
        }

        public async Task<EvaluateSpecFlowFeatureFileResponse> EvaluateSpecFlowFeatureFileAsync(string requirements, string featureFile, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting {MethodName}.", nameof(EvaluateSpecFlowFeatureFileAsync));
            ValidateInput(requirements, "requirements");
            ValidateInput(featureFile, "feature file");

            evaluateSpecFlowFeatureFileGenerator.FeatureFile = featureFile;
            evaluateSpecFlowFeatureFileGenerator.Requirements = requirements;
            SelectedGenerator = evaluateSpecFlowFeatureFileGenerator;

            _logger.LogInformation("Generator {GeneratorName} selected for SpecFlow feature file evaluation.", nameof(EvaluateSpecFlowFeatureFileGenerator));
            return await EvaluateAsync<GetSpecFlowFeatureFileEvaluationResponse, EvaluateSpecFlowFeatureFileResponse>("SpecFlow feature file evaluation failed.", cancellationToken);
        }

        public async Task<EvaluateSpecFlowScenarioResponse> EvaluateSpecFlowScenarioAsync(string requirements, string featureFile, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting {MethodName}.", nameof(EvaluateSpecFlowScenarioAsync));
            ValidateInput(requirements, "requirements");
            ValidateInput(featureFile, "feature file");

            evaluateSpecFlowScenarioGenerator.FeatureFile = featureFile;
            evaluateSpecFlowScenarioGenerator.Requirements = requirements;
            SelectedGenerator = evaluateSpecFlowScenarioGenerator;

            _logger.LogInformation("Generator {GeneratorName} selected for SpecFlow scenario evaluation.", nameof(EvaluateSpecFlowScenarioGenerator));
            return await EvaluateAsync<GetSpecFlowScenarioEvaluationResponse, EvaluateSpecFlowScenarioResponse>("SpecFlow scenario evaluation failed.", cancellationToken);
        }

        public async Task<SpecFlowBindingFileResponse> GenerateSpecFlowBindingFileAsync(string featureFile, Dictionary<string, string> generatedCSharpCode, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting {MethodName}.", nameof(GenerateSpecFlowBindingFileAsync));

            ValidateInput(featureFile, "feature file");
            _logger.LogInformation("Feature file validated successfully.");

            generatedCSharpCode ??= [];

            specFlowBindingFileGenerator.FeatureFile = featureFile;
            specFlowBindingFileGenerator.GeneratedCSharpCode = generatedCSharpCode;
            SelectedGenerator = specFlowBindingFileGenerator;

            var response = await GenerateAsync(cancellationToken);
            _logger.LogInformation("Response successfully generated by {LLMName}.", SelectedLLM.ToString() ?? "Unknown LLM");

            _logger.LogInformation("Converting response JSON to SpecFlow binding file object.");
            var specFlowBindingFile = GetSpecFlowBindingFileResponse.ConvertJson(response.Response.Messages.Last().Text);
            specFlowBindingFile.RawResponse.AddRange(response.Response.Messages.Select(m => m.Text));
            _logger.LogInformation("SpecFlow binding file conversion completed successfully.");

            UnescapeDictionary(specFlowBindingFile.BindingFiles);
            specFlowBindingFile.Prompts = response.Request.Messages.Select(m => m.Content.Text).ToList();

            _logger.LogInformation("{MethodName} completed successfully.", nameof(GenerateSpecFlowBindingFileAsync));
            return specFlowBindingFile;
        }

        public async Task<RequirementResponse> GenerateRequirementAsync(Dictionary<string, string> testFiles, string existingRequirement = "", CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting requirement generation.");

            requirementGenerator.TestFiles = testFiles;
            requirementGenerator.ExistingRequirement = existingRequirement;
            SelectedGenerator = requirementGenerator;

            var response = await GenerateAsync(cancellationToken);
            _logger.LogInformation("Response successfully generated.");
            _logger.LogInformation("Converting response YAML to requirement model.");
            var requirementResponse = GetRequirementResponse.ConvertYaml(response.Response.Messages.Last().Text);
            requirementResponse.RawResponse.AddRange(response.Response.Messages.Select(m => m.Text));
            _logger.LogInformation("Requirement conversion completed successfully.");
            requirementResponse.Response = response.Response.Messages.Last().Text;
            requirementResponse.Prompts = response.Request.Messages.Select(m => m.Content.Text).ToList();
            return requirementResponse;
        }

        public async Task<IEnumerable<string>> GetAllReqIFFilesAsync()
        {
            _logger.LogInformation("Getting all ReqIF files.");
            var files = await _reqIFStorage.GetAllReqIFsAsync();
            _logger.LogInformation("Successfully fetched all ReqIF files.");

            return files;
        }

        public string GetStorageLocation()
        {
            return _reqIFStorage.GitLocationPath;
        }

        public void UpdateStorageLocation(string newPath)
        {
            _reqIFStorage.GitLocationPath = newPath;
        }

        private async Task<GeneratorResponse> GenerateAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting response generation using {GeneratorName} with {LLMName}.", SelectedGenerator?.GetType().Name ?? "Unknown Generator", SelectedLLM.ToString() ?? "Unknown LLM");
            CheckLLM();
            CheckGenerator();

            var response = await SelectedGenerator!.GenerateAsync(_largeLanguageModel, (ModelName)SelectedLLM!, cancellationToken);
            _logger.LogInformation("Response generation completed successfully.");
            return response;
        }

        private async Task<TResponse> EvaluateAsync<TConverter, TResponse>(string errorMessage, CancellationToken cancellationToken = default)
        where TConverter : class
        {
            _logger.LogInformation("Starting evaluation process.");
            var responseJson = await GenerateAsync(cancellationToken);
            _logger.LogInformation("Response JSON successfully generated.");

            try
            {
                _logger.LogInformation("Converting response JSON to {ResponseType}.", typeof(TResponse).Name);
                var response = (TResponse)typeof(TConverter).GetMethod("ConvertJson")!.Invoke(null, [responseJson.Response.Messages.Last().Text])!;
                _logger.LogInformation("Response JSON successfully converted to {ResponseType}.", typeof(TResponse).Name);

                response.GetType().GetProperty("Prompts")!.SetValue(response, responseJson.Request.Messages.Select(m => m.Content.Text).ToList());
                var rawResponseProperty = response.GetType().GetProperty("RawResponse");
                if (rawResponseProperty != null)
                {
                    if (rawResponseProperty.GetValue(response) is List<string> rawResponseValue)
                    {
                        rawResponseValue.AddRange(responseJson.Response.Messages.Select(m => m.Text));

                        rawResponseProperty.SetValue(response, rawResponseValue);
                    }
                    else
                    {
                        var newRawResponse = new List<string> { responseJson.Response.Messages.Last().Text };
                        rawResponseProperty.SetValue(response, newRawResponse);
                    }
                }
                else
                {
                    throw new InvalidOperationException("RawResponse property not found on the response object.");
                }

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

        private Dictionary<string, string> UnescapeDictionary(Dictionary<string, string> dictionary)
        {
            try
            {
                foreach (var item in dictionary)
                {
                    dictionary[item.Key] = Regex.Unescape(item.Value);
                }
                return dictionary;
            }

            catch (RegexParseException)
            {
                _logger.LogWarning("Encountered regex parse error.");
                return dictionary;
            }
            catch
            {
                throw;
            }
        }
    }

}
