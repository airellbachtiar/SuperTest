using OpenAI;
using OpenAI.Chat;

namespace SuperTestLibrary.LLMs
{
    public class GPT_4o : ILargeLanguageModel
    {
        private const string GPT_4o_Model = "gpt-4o";

        private static readonly OpenAIClient _openAIClient;

        public const string ModelName = "GPT-4o";

        public string Id => ModelName;

        static GPT_4o()
        {
            string? ApiKey = Environment.GetEnvironmentVariable("SUPERTEST_OPENAI_API_KEY", EnvironmentVariableTarget.User) ?? throw new InvalidOperationException("SUPERTEST_OPENAI_API_KEY is not set.");

            _openAIClient = new OpenAIClient(ApiKey);
            ApiKey = null;
        }

        public async Task<string> Call(IEnumerable<string> messages)
        {
            List<ChatMessage> prompts = new List<ChatMessage>();

            foreach (var messageContent in messages)
            {
                prompts.Add(new UserChatMessage(messageContent));
            }

            var response = await _openAIClient.GetChatClient(GPT_4o_Model).CompleteChatAsync(prompts);

            return response.Value.Content.First().Text ?? string.Empty;
        }
    }
}
