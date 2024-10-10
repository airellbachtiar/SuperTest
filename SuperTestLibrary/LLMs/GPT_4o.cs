using DotNetEnv;
using OpenAI;
using OpenAI.Chat;
using SuperTestLibrary.LLMs.PromptBuilders;
using System.Text.Json;

namespace SuperTestLibrary.LLMs
{
    public class GPT_4o : ILargeLanguageModel
    {
        private class GPT_4o_Settings
        {
            public Prompt? GenerateFeatureFile { get; init; }
        }

        private const string SettingFile = "LLMs/Settings/GPT_4o.json";
        private const string GPT_4o_Model = "gpt-4o";

        private static readonly GPT_4o_Settings _settings;
        private static readonly OpenAIClient _openAIClient;

        public const string ModelName = "GPT-4o";

        private readonly IPromptBuilder _promptBuilder;

        static GPT_4o()
        {
            Env.Load();
            string? ApiKey = Env.GetString("OPENAI_API_KEY") ?? throw new InvalidOperationException("OPENAI_API_KEY is not set.");

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

            _openAIClient = new OpenAIClient(ApiKey);
            ApiKey = null;
        }

        public GPT_4o(IPromptBuilder promptBuilder)
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

            List<ChatMessage> messages = new List<ChatMessage>();

            foreach (var prompt in prompts)
            {
                messages.Add(new UserChatMessage(prompt));
            }

            var message = await _openAIClient.GetChatClient(GPT_4o_Model).CompleteChatAsync(messages);

            return message.Value.Content.ToString() ?? string.Empty;
        }
    }
}
