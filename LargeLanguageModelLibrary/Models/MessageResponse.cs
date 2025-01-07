namespace LargeLanguageModelLibrary.Models
{
    public class MessageResponse
    {
        public ChatMessageContent[] Messages { get; set; }
        public string ModelName { get; set; }
        public string Id { get; set; }
    }
}
