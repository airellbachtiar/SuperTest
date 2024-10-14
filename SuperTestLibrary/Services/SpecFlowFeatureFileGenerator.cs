using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services.Prompts;
using SuperTestLibrary.Services.Prompts.Builders;
using System.Text.Json;

namespace SuperTestLibrary.Services
{
    public class SpecFlowFeatureFileGenerator : IGenerator
    {
        private ILargeLanguageModel? _llm;
        private string _requirements = string.Empty;

        private const string _jsonPromptClaude_3_5_Sonnet = "Services/Prompts/SpecFlowFeatureFileClaude_3_5_Sonnet.json";
        private const string _jsonPromptGPT_4o = "Services/Prompts/SpecFlowFeatureFileGPT_4o.json";

        public async Task<SpecFlowFeatureFileResponse> Generate(ILargeLanguageModel largeLanguageModel, string requirements)
        {
            _llm = largeLanguageModel;
            _requirements = requirements;

            string jsonPromptPath = string.Empty;
            jsonPromptPath = _llm switch
            {
                Claude_3_5_Sonnet => jsonPromptPath = _jsonPromptClaude_3_5_Sonnet,
                GPT_4o => jsonPromptPath = _jsonPromptGPT_4o,
                Gemini_1_5 => throw new InvalidOperationException("Gemini 1.5 does not support generating SpecFlow feature files."),
                _ => throw new InvalidOperationException("Unknown LLM."),
            };

            IEnumerable<string> prompts = SetupPrompt(jsonPromptPath);
            var response = await _llm.Call(prompts);

            return GetSpecFlowFeatureFiles(response);
        }

        private IEnumerable<string> SetupPrompt(string jsonPromptPath)
        {
            using var fs = File.OpenRead(jsonPromptPath) ?? throw new FileNotFoundException($"Unable to locate generate SpecFlow feature file prompts for {_llm}.");
            Prompt prompt = JsonSerializer.Deserialize<Prompt>(fs)! ?? throw new InvalidOperationException("Unable to read generate SpecFlow feature file prompts for Claude 3.5 Sonnet.");

            IPromptBuilder promptBuilder = new SpecFlowFeatureFilePromptBuilder();

            var prompts = promptBuilder.BuildPrompt(prompt, _requirements);

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
    }
}
