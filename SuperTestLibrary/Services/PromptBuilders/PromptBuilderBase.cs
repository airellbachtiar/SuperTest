using LargeLanguageModelLibrary.Models;
using SuperTestLibrary.Helpers;

namespace SuperTestLibrary.Services.PromptBuilders
{
    public abstract class PromptBuilderBase : IPromptBuilder
    {
        protected Prompt? _prompt;

        public MessageRequest BuildPrompt(Prompt prompt)
        {
            ArgumentNullException.ThrowIfNull(prompt);
            _prompt = prompt;

            var prompts = new List<string> { BuildContext() };

            if (prompt.Interactions.Any())
            {
                prompts.AddRange(GetListOfInteractions.BuildInteractions(prompt));
            }

            MessageRequest request = new();

            foreach (var promptStr in prompts)
            {
                request.Messages.Add(ChatMessage.CreateUserChatMessage(promptStr));
            }

            return request;
        }

        protected abstract string BuildContext();
    }
}
