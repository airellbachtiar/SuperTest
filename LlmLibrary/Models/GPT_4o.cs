using OpenAI;
using OpenAI.Chat;

namespace LlmLibrary.Models
{
    public class GPT_4o : ILargeLanguageModel
    {
        private const string GPT_4o_Model = "chatgpt-4o-latest";

        private readonly OpenAIClient _openAIClient;

        public const string ModelName = "GPT-4o";

        public string Id => ModelName;

        public GPT_4o()
        {
            string? ApiKey = Environment.GetEnvironmentVariable("SUPERTEST_OPENAI_API_KEY", EnvironmentVariableTarget.User) ?? throw new InvalidOperationException("SUPERTEST_OPENAI_API_KEY is not set.");

            _openAIClient = new OpenAIClient(ApiKey);
        }

        public async Task<string> CallAsync(IEnumerable<string> messages)
        {
            List<ChatMessage> prompts = [];

            foreach (var messageContent in messages)
            {
                prompts.Add(new UserChatMessage(messageContent));
            }

            var response = await _openAIClient.GetChatClient(GPT_4o_Model).CompleteChatAsync(prompts, new ChatCompletionOptions());

            return response.Value.Content.First().Text ?? string.Empty;
        }
    }
}
