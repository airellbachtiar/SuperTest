namespace SuperTestLibrary.LLMs
{
    public class Prompt
    {
        public string SystemInstruction { get; init; } = string.Empty;
        public IEnumerable<string> Instructions { get; init; } = Array.Empty<string>();
        public string Thinking { get; init; } = string.Empty;
        public string Example { get; init; } = string.Empty;
    }
}
