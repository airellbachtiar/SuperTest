namespace SuperTestLibrary.Services.Prompts.Builders
{
    public interface IPromptBuilder
    {
        IEnumerable<string> BuildPrompt(Prompt prompt);
    }
}