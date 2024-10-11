namespace SuperTestLibrary.LLMs.PromptBuilders
{
    public interface IPromptBuilder
    {
        string BuildPrompt(Prompt prompt, string requirements);
    }
}