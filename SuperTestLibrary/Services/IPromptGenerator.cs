namespace SuperTestLibrary.Services
{
    public interface IPromptGenerator
    {
        Task<string> GenerateFile();
    }
}
