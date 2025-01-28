using LargeLanguageModelLibrary.Models;

namespace SuperTestLibrary.Models
{
    public class GeneratorResponse (MessageResponse response, MessageRequest request)
    {
        public MessageResponse Response { get; } = response;
        public MessageRequest Request { get; } = request;
    }
}
