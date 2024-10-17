namespace SuperTestLibrary.LLMs.PromptBuilders
{
    public interface IPromptBuilder
    {
        IEnumerable<string> BuildPrompt(Prompt prompts, string requirements);
    }
}