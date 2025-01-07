namespace LargeLanguageModelLibrary.Models
{
    public class ChatMessageContent(string text, ImageContent imageUri)
    {
        private readonly string _text = text;
        private readonly ImageContent _imageUri = imageUri;

        public string Text => _text;
        public ImageContent ImageUri => _imageUri;

        public static ChatMessageContent CreateTextMessage(string text)
        {
            return new ChatMessageContent(text, null);
        }

        public static ChatMessageContent CreateImageMessage(ImageContent imageUri, string text = null)
        {
            return new ChatMessageContent(text, imageUri);
        }
    }
}
