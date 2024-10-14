using DotNetEnv;
using OpenAI;
using OpenAI.Chat;
using SuperTestLibrary.Services.Prompts;
using System.ClientModel;
using System.Text.Json;

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
            Env.Load();
            string? ApiKey = Env.GetString("OPENAI_API_KEY") ?? throw new InvalidOperationException("OPENAI_API_KEY is not set.");

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

            return GetSpecFlowFeatureFiles(response).FirstOrDefault() ?? string.Empty;
        }

        private static IEnumerable<string> GetSpecFlowFeatureFiles(ClientResult<ChatCompletion>? messageResponse)
        {
            if (messageResponse != null)
            {
                var response = JsonSerializer.Deserialize<SpecFlowFeatureFileResponse>(messageResponse.Value.Content.First().Text);

                if (response != null)
                {
                    return response.FeatureFiles.Values;
                }
            }

            return Enumerable.Empty<string>();
        }
    }
}
