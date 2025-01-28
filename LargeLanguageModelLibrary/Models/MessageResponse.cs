namespace LargeLanguageModelLibrary.Models
{
    public class MessageResponse
    {
        public List<MessageResponseContent> Messages { get; set; } = [];
        public string ModelName { get; set; }
        public string Id { get; set; }
    }
}
