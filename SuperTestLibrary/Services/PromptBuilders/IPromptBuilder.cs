namespace SuperTestLibrary.Services.PromptBuilders
{
    public interface IPromptBuilder
    {
        IEnumerable<string> BuildPrompt(Prompt prompt);
    }
}