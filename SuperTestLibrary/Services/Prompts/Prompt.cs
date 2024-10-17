namespace SuperTestLibrary.Services.Prompts
{
    public class Prompt
    {
        public string SystemInstruction { get; init; } = string.Empty;
        public IEnumerable<string> Instructions { get; init; } = [];
        public string Thinking { get; init; } = string.Empty;
        public string Example { get; init; } = string.Empty;
        public IEnumerable<Interaction> Interactions { get; init; } = [];
    }
}
