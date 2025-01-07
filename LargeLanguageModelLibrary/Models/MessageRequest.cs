namespace LargeLanguageModelLibrary.Models
{
    public class MessageRequest
    {
        public string Model { get; set; }
        public double MaxTokens { get; set; }
        public double Temperature { get; set; }
        public double TopP { get; set; }
        public double TopK { get; set; }
        public ChatMessage[] Messages { get; set; }
    }
}
