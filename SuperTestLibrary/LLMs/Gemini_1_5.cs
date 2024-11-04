using GenerativeAI.Models;
using GenerativeAI.Types;

namespace SuperTestLibrary.LLMs
{
    public class Gemini_1_5 : ILargeLanguageModel
    {
        private static readonly GenerativeModel _gemini;

        public const string ModelName = "Gemini 1.5";

        public string Id => ModelName;

        static Gemini_1_5()
        {
            string? ApiKey = Environment.GetEnvironmentVariable("SUPERTEST_GEMINI_API_KEY", EnvironmentVariableTarget.User) ?? throw new InvalidOperationException("SUPERTEST_GEMINI_API_KEY is not set.");

            _gemini = new GeminiProModel(ApiKey)
            {
                Config =
                {
                    MaxOutputTokens = 20000
                }
            };
            ApiKey = null;
        }

        public async Task<string> CallAsync(IEnumerable<string> messages)
        {
            var chat = _gemini.StartChat(new StartChatParams());

            string response = string.Empty;

            foreach (var prompt in messages)
            {
                response = await chat.SendMessageAsync(prompt);
            }

            return response;
        }
    }
}
