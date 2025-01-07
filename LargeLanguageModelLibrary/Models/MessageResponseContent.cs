namespace LargeLanguageModelLibrary.Models
{
    public class MessageResponseContent
    {
        public string Text { get; set; }
        public TokenUsage TokenUsage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
