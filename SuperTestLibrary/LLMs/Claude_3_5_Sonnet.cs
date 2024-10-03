using Claudia;
using System.Text;
using System.Text.Json;

namespace SuperTestLibrary.LLMs
{
    public class Claude_3_5_Sonnet : ILargeLanguageModel
    {
        private class Claude_3_5_SonnetSettings
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

        private const string SettingFile = "LLMs/Settings/Claude_3_5_Sonnet.json";
        private const string Claude_3_5_SonnetModel = "claude-3-5-sonnet-20240620";

        private static readonly Claude_3_5_SonnetSettings _settings;
        private static readonly Anthropic _anthropic;

        public const string ModelName = "Claude 3.5 Sonnet";

        static Claude_3_5_Sonnet()
        {
            using var fs = File.OpenRead(SettingFile) ?? throw new InvalidOperationException($"Unable to locate settings from {SettingFile}.");
            try
            {
                _settings = JsonSerializer.Deserialize<Claude_3_5_SonnetSettings>(fs)!;
            }
            catch { }

            if (_settings == null)
            {
                throw new InvalidOperationException($"Unable to read settings from {SettingFile}, unable to initialize communication with LLM.");
            }

            _anthropic = new Anthropic
            {
                ApiKey = _settings.ApiKey!
            };
        }

        public async Task<string> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            if (_settings.GenerateFeatureFile == null)
            {
                throw new InvalidOperationException("GenerateFeatureFile prompt is not set.");
            }

            var prompt = PromptBuilder(_settings.GenerateFeatureFile, requirements);

            var message = await _anthropic.Messages.CreateAsync(new()
            {
                Model = Claude_3_5_SonnetModel,
                MaxTokens = 1024,
                Messages = [new() { Role = "user", Content = prompt }]
            });

            return message.Content.ToString();
        }

        private string PromptBuilder(Prompt prompt, string requirements)
        {
            var promptBuilder = new StringBuilder();

            promptBuilder.AppendLine(prompt.SystemInstruction);
            promptBuilder.AppendLine();

            foreach (var instruction in prompt.Instructions.Select((value, i) => new { i, value }))
            {
                promptBuilder.AppendLine($"{instruction.i}. {instruction.value}");
            }

            promptBuilder.AppendLine();
            promptBuilder.AppendLine(prompt.Thinking);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine(prompt.Example);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("<Requirements>");
            promptBuilder.AppendLine(requirements);
            promptBuilder.AppendLine("</Requirements>");

            return promptBuilder.ToString();
        }
    }
}
