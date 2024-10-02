using Claudia;
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
            public string UserPrompt { get; init; } = string.Empty;
        }

        private const string ApiKeyFile = "Claude_3_5_Sonnet.json";
        private const string Claude_3_5_SonnetModel = "claude-3-5-sonnet-20240620";

        private static readonly Claude_3_5_SonnetSettings _settings;
        private static readonly Anthropic _anthropic;

        static Claude_3_5_Sonnet()
        {
            using var fs = File.OpenRead(ApiKeyFile);
            try
            {
                _settings = JsonSerializer.Deserialize<Claude_3_5_SonnetSettings>(fs)!;
            }
            catch { }

            if (_settings == null)
            {
                throw new InvalidOperationException($"Unable to read settings from {ApiKeyFile}, unable to initialize communication with LLM.");
            }

            _anthropic = new Anthropic
            {
                ApiKey = _settings.ApiKey!
            };
        }

        public async Task<string> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            var prompt = $"{_settings.GenerateFeatureFile!.UserPrompt}\n<Requirements>\n{requirements}\n</Requirements>";
            //prompt = "Who are you?";

            var message = await _anthropic.Messages.CreateAsync(new()
            {
                Model = Claude_3_5_SonnetModel,
                MaxTokens = 1024,
                Messages = [new() { Role = "user", Content = prompt }]
            });

            return message.Content.ToString();
        }
    }
}
