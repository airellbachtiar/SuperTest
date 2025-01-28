using LargeLanguageModelLibrary.Models;

namespace SuperTestLibrary.Services.PromptBuilders
{
    public interface IPromptBuilder
    {
        MessageRequest BuildPrompt(Prompt prompt);
    }
}