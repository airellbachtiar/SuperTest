using SuperTestLibrary.Services.Prompts;
using System.Text.Json;

namespace SuperTestLibrary.Helpers
{
    public static class GetPromptFromJson
    {
        public static Prompt ConvertJson(string jsonPromptPath)
        {
            using var fs = File.OpenRead(jsonPromptPath) ?? throw new FileNotFoundException($"Unable to locate {jsonPromptPath}.");
            Prompt prompt = JsonSerializer.Deserialize<Prompt>(fs)! ?? throw new InvalidOperationException($"Unable to read {jsonPromptPath}");

            return prompt;
        }
    }
}
