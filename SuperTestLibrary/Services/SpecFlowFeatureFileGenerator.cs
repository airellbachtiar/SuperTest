namespace SuperTestLibrary.Services
{
    public class SpecFlowFeatureFileGenerator : IGenerator
    {
        private ILargeLanguageModel? _llm;
        private string _requirements = string.Empty;

        private const string _jsonPromptClaude_3_5_Sonnet = "Services/Prompts/SpecFlowFeatureFileClaude_3_5_Sonnet.json";
        private const string _jsonPromptGPT_4o = "Services/Prompts/SpecFlowFeatureFileGPT_4o.json";
        private const string _jsonPromptGemini_1_5 = "Services/Prompts/SpecFlowFeatureFileGemini_1_5.json";

        public async Task<string> Generate(ILargeLanguageModel largeLanguageModel, string requirements)
        {
            _llm = largeLanguageModel;
            _requirements = requirements;

            string jsonPromptPath = _llm.Id switch
            {
                "Claude 3.5 Sonnet" => _jsonPromptClaude_3_5_Sonnet,
                "GPT-4o" => _jsonPromptGPT_4o,
                "Gemini 1.5" => _jsonPromptGemini_1_5,
                _ => throw new InvalidOperationException("Unknown LLM."),
            };

            IEnumerable<string> prompts = SetupPrompt(jsonPromptPath);
            string response = await _llm.Call(prompts);

            var specFlowFeatureFile = GetSpecFlowFeatureFiles(response);

            if (ValidateFeatureFile(specFlowFeatureFile))
            {
                return specFlowFeatureFile;
            }
            else
            {
                throw new InvalidOperationException("Invalid SpecFlow feature file generated.");
            }
            if (string.IsNullOrEmpty(response))
            {
                throw new InvalidOperationException("Unable to generate SpecFlow feature file.");
            }
            return response;
        }

        private IEnumerable<string> SetupPrompt(string jsonPromptPath)
        {
            using var fs = File.OpenRead(jsonPromptPath) ?? throw new FileNotFoundException($"Unable to locate generate SpecFlow feature file prompts for {_llm}.");
            Prompt prompt = JsonSerializer.Deserialize<Prompt>(fs)! ?? throw new InvalidOperationException("Unable to read generate SpecFlow feature file prompts for Claude 3.5 Sonnet.");

            var prompts = new SpecFlowFeatureFilePromptBuilder().BuildPrompt(prompt, _requirements);

            return prompts;
        }

        private SpecFlowFeatureFileResponse GetSpecFlowFeatureFiles(string response)
        {
            var specFlowFeatureFiles = JsonSerializer.Deserialize<SpecFlowFeatureFileResponse>(response);

            if (specFlowFeatureFiles != null)
            {
                return specFlowFeatureFiles;
            }
            else return new SpecFlowFeatureFileResponse();
        }

        public bool ValidateFeatureFile(SpecFlowFeatureFileResponse response)
        {
            var parser = new Parser();
            try
            {
                foreach (var featureFile in response.FeatureFiles)
                {
                    var gherkinDocument = parser.Parse(new StringReader(featureFile.Value));

                    // If no exception is thrown, the feature file is valid.
                    if (gherkinDocument.Feature == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
