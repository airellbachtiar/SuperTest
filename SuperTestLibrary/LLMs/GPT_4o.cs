using OpenAI;
using System.Text;
using System.Text.Json;

namespace SuperTestLibrary.LLMs
{
    public class GPT_4o : ILargeLanguageModel
    {
        private class GPT_4o_Settings
        {
            public string? ApiKey { get; init; }
            public Prompt? GenerateFeatureFile { get; init; }
        }

        private class Prompt
        {
            public string SystemInstruction { get; init; } = string.Empty;
            public IEnumerable<string> Instructions { get; init; } = Array.Empty<string>();
            public string Thinking { get; init; } = string.Empty;
            public string Example { get; init; } = string.Empty;
        }

        private const string SettingFile = "LLMs/Settings/GPT_4o.json";
        private const string GPT_4o_Model = "gpt-4o";

        private static readonly GPT_4o_Settings _settings;
        private static readonly OpenAIClient _openAIClient;

        public const string ModelName = "GPT-4o";

        static GPT_4o()
        {
            using var fs = File.OpenRead(SettingFile) ?? throw new InvalidOperationException($"Unable to locate settings from {SettingFile}.");
            try
            {
                _settings = JsonSerializer.Deserialize<GPT_4o_Settings>(fs)!;
            }
            catch { }

            if (_settings == null)
            {
                throw new InvalidOperationException($"Unable to read settings from {SettingFile}, unable to initialize communication with LLM.");
            }

            _openAIClient = new OpenAIClient(_settings.ApiKey!);
        }

        public async Task<string> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            if (_settings.GenerateFeatureFile == null)
            {
                throw new InvalidOperationException("GenerateFeatureFile prompt is not set.");
            }

            var prompt = PromptBuilder(_settings.GenerateFeatureFile, requirements);

            var message = _openAIClient.GetChatClient(GPT_4o_Model).CompleteChatAsync(prompt);

            return string.Empty;
        }

        private string PromptBuilder(Prompt prompt, string requirements)
        {
            var sb = new StringBuilder();

            sb.AppendLine(prompt.SystemInstruction);
            sb.AppendLine();

            foreach (var instruction in prompt.Instructions)
            {
                sb.AppendLine(instruction);
            }

            sb.AppendLine();
            sb.AppendLine(prompt.Thinking);
            sb.AppendLine();
            sb.AppendLine(requirements);
            sb.AppendLine();
            sb.AppendLine(prompt.Example);

            return sb.ToString();
        }
    }
}
