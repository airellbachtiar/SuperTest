using Claudia;

namespace LlmLibrary.Models
{
    public class Claude_3_5_Sonnet : ILargeLanguageModel
    {
        private const string Claude_3_5_SonnetModel = "claude-3-5-sonnet-20240620";

        private readonly Anthropic _anthropic;

        public const string ModelName = "Claude 3.5 Sonnet";

        public string Id => ModelName;

        public Claude_3_5_Sonnet()
        {
            string? ApiKey = Environment.GetEnvironmentVariable("SUPERTEST_ANTHROPIC_API_KEY", EnvironmentVariableTarget.User) ?? throw new InvalidOperationException("SUPERTEST_ANTHROPIC_API_KEY is not set.");

            _anthropic = new Anthropic
            {
                ApiKey = ApiKey!
            };
        }

        public async Task<string> CallAsync(IEnumerable<string> messages, CancellationToken cancellationToken = default)
        {
            List<Message> prompts = [];

            foreach (var messageContent in messages)
            {
                prompts.Add(new()
                {
                    Role = "user",
                    Content = messageContent
                });
            }

            var message = await _anthropic.Messages.CreateAsync(new()
            {
                Model = Claude_3_5_SonnetModel,
                MaxTokens = 8192,
                Messages = prompts.ToArray()
            },
            null,
            cancellationToken);

            return message.Content.ToString() ?? string.Empty;
        }
    }
}
