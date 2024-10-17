using Claudia;
using DotNetEnv;
using SuperTestLibrary.LLMs.PromptBuilders;
using System.Text.Json;

namespace SuperTestLibrary.LLMs
{
    public class Claude_3_5_Sonnet : ILargeLanguageModel
    {
        private class Claude_3_5_SonnetSettings
        {
            public Prompt? GenerateFeatureFile { get; init; }
        }

        private const string SettingFile = "LLMs/Settings/Claude_3_5_Sonnet.json";
        private const string Claude_3_5_SonnetModel = "claude-3-5-sonnet-20240620";

        private static readonly Claude_3_5_SonnetSettings _settings;
        private static readonly Anthropic _anthropic;

        public const string ModelName = "Claude 3.5 Sonnet";

        private readonly IPromptBuilder _promptBuilder;

        static Claude_3_5_Sonnet()
        {
            Env.Load();
            string? ApiKey = Env.GetString("ANTHROPIC_API_KEY") ?? throw new InvalidOperationException("ANTHROPIC_API_KEY is not set.");

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
                ApiKey = ApiKey!
            };

            ApiKey = null;
        }

        public Claude_3_5_Sonnet(IPromptBuilder promptBuilder)
        {
            _promptBuilder = promptBuilder;
        }

        public async Task<string> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            if (_settings.GenerateFeatureFile == null)
            {
                throw new InvalidOperationException("GenerateFeatureFile prompt is not set.");
            }

            var prompts = _promptBuilder.BuildPrompt(_settings.GenerateFeatureFile, requirements);

            List<Message> messages = new List<Message>();

            foreach (var prompt in prompts)
            {
                messages.Add(new()
                {
                    Role = "user",
                    Content = prompt
                });
            }

            var message = await _anthropic.Messages.CreateAsync(new()
            {
                Model = Claude_3_5_SonnetModel,
                MaxTokens = 1024,
                Messages = messages.ToArray()
            });

            return message.Content.ToString();
        }
    }
}
