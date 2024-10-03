using GenerativeAI.Models;
using GenerativeAI.Types;
using System.Text;
using System.Text.Json;

namespace SuperTestLibrary.LLMs
{
    public class Gemini_1_5 : ILargeLanguageModel
    {
        private class Gemini_1_5_Settings
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

        private const string SettingFile = "LLMs/Settings/Gemini_1_5.json";

        private static readonly Gemini_1_5_Settings _settings;
        private static readonly GenerativeModel _gemini;

        public const string ModelName = "Gemini 1.5";

        static Gemini_1_5()
        {
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

            _gemini = new GenerativeModel(_settings.ApiKey!);
        }

        public async Task<string> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            if (_settings.GenerateFeatureFile == null)
            {
                throw new InvalidOperationException("GenerateFeatureFile prompt is not set.");
            }

            var prompt = PromptBuilder(_settings.GenerateFeatureFile, requirements);

            var chat = _gemini.StartChat(new StartChatParams());

            var response = await chat.SendMessageAsync(prompt);

            return response;
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
