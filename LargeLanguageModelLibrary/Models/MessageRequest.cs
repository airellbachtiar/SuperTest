namespace LargeLanguageModelLibrary.Models
{
    public class MessageRequest
    {
        public string Model { get; set; }
        public int? MaxTokens { get; set; } = null;
        public double? Temperature { get; set; } = null;
        public double? TopP { get; set; } = null;
        public int? TopK { get; set; } = null;
        public List<ChatMessage> Messages { get; set; } = [];
    }
}