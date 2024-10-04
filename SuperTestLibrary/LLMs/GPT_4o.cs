using DotNetEnv;
using OpenAI;
using System.Text;
using System.Text.Json;

namespace SuperTestLibrary.LLMs
{
    public class GPT_4o : ILargeLanguageModel
    {
        private class GPT_4o_Settings
        {
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

        public async Task<string> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            if (_settings.GenerateFeatureFile == null)
            {
                throw new InvalidOperationException("GenerateFeatureFile prompt is not set.");
            }

            var prompt = PromptBuilder(_settings.GenerateFeatureFile, requirements);

            var message = await _openAIClient.GetChatClient(GPT_4o_Model).CompleteChatAsync(prompt);

            return message.Value.Content.ToString() ?? string.Empty;
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
