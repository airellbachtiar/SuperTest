namespace SuperTestLibrary.Models
{
    public class GeneratorResponse (string response, IEnumerable<string> prompts)
    {
        public string ResponseString { get; } = response;
        public IEnumerable<string> Prompts { get; } = prompts;
    }
}
