using GenerativeAI.Models;
using GenerativeAI.Types;
using System.Text.Json;

namespace SuperTestLibrary.Services
{
    public class Gemini1_5 : ILLM
    {
        private class Gemini1_5Settings
        {
            public string? ApiKey { get; init; }
            public Prompt? GenerateFeatureFile { get; init; }
        }

        private class Prompt
        {
            public string SystemInstruction { get; init; } = string.Empty;
            public string UserPrompt { get; init; } = string.Empty;
        }

        private const string ApiKeyFile = "Gemini1_5Flash.json";

        private static readonly Gemini1_5Settings _settings;
        private static readonly GenerativeModel _gemini;

        static Gemini1_5()
        {
            using var fs = File.OpenRead(ApiKeyFile);
            try
            {
                _settings = JsonSerializer.Deserialize<Gemini1_5Settings>(fs)!;
            }
            catch { }

            if (_settings == null)
            {
                throw new InvalidOperationException($"Unable to read settings from {ApiKeyFile}, unable to initialize communication with LLM.");
            }

            _gemini = new GenerativeModel(_settings.ApiKey!);
        }

        public Gemini1_5()
        {
        }

        public async Task<string> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            //TODO: Input requirements
            var chat = _gemini.StartChat(new StartChatParams());

            var prompt = $"{_settings.GenerateFeatureFile!.UserPrompt}\n<Requirements>\n{requirements}\n</Requirements>";

            var response = await chat.SendMessageAsync(prompt);

            return response;
        }
    }
}
