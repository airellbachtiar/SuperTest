using DotNetEnv;
using GenerativeAI.Models;
using GenerativeAI.Types;
using SuperTestLibrary.LLMs.PromptBuilders;
using System.Text.Json;

namespace SuperTestLibrary.LLMs
{
    public class Gemini_1_5 : ILargeLanguageModel
    {
        private class Gemini_1_5_Settings
        {
            public Prompt? GenerateFeatureFile { get; init; }
        }

        private const string SettingFile = "LLMs/Settings/Gemini_1_5.json";

        private static readonly Gemini_1_5_Settings _settings;
        private static readonly GenerativeModel _gemini;

        public const string ModelName = "Gemini 1.5";

        private readonly IPromptBuilder _promptBuilder;

        static Gemini_1_5()
        {
            Env.Load();
            string? ApiKey = Env.GetString("GEMINI_API_KEY") ?? throw new InvalidOperationException("GEMINI_API_KEY is not set.");

            using var fs = File.OpenRead(SettingFile) ?? throw new InvalidOperationException($"Unable to locate settings from {SettingFile}.");
            try
            {
                _settings = JsonSerializer.Deserialize<Gemini_1_5_Settings>(fs)!;
            }
            catch { }

            if (_settings == null)
            {
                throw new InvalidOperationException($"Unable to read settings from {SettingFile}, unable to initialize communication with LLM.");
            }

            _gemini = new GenerativeModel(ApiKey);
            ApiKey = null;
        }

        public Gemini_1_5(IPromptBuilder promptBuilder)
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

            var chat = _gemini.StartChat(new StartChatParams());

            string response = string.Empty;

            foreach (var prompt in prompts)
            {
                response = await chat.SendMessageAsync(prompt);
            }

            return response;
        }
    }
}
