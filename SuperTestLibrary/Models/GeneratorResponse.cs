namespace SuperTestLibrary.Models
{
    public class GeneratorResponse (string response, IEnumerable<string> prompts)
    {
        public string ResponseJson { get; } = response;
        public IEnumerable<string> Prompts { get; } = prompts;
    }
}
