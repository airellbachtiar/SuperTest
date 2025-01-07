using LargeLanguageModelLibrary.Enums;

namespace LargeLanguageModelLibrary.Models
{
    public class ChatMessage
    {
        public ChatMessageContent Content { get; set; }
        public ChatMessageRole Role { get; set; }

        #region Create Developer/System Chat Message
        public static ChatMessage CreateDeveloperChatMessage(ChatMessageContent message)
        {
            return new ChatMessage
            {
                Role = ChatMessageRole.Developer,
                Content = message
            };
        }

        public static ChatMessage CreateDeveloperChatMessage(string message)
        {
            return new ChatMessage
            {
                Role = ChatMessageRole.Developer,
                Content = ChatMessageContent.CreateTextMessage(message)
            };
        }

        public static ChatMessage CreateSystemChatMessage(ChatMessageContent message)
        {
            return new ChatMessage
            {
                Role = ChatMessageRole.Developer,
                Content = message
            };
        }

        public static ChatMessage CreateSystemChatMessage(string message)
        {
            return new ChatMessage
            {
                Role = ChatMessageRole.Developer,
                Content = ChatMessageContent.CreateTextMessage(message)
            };
        }
        #endregion

        #region Create User Chat Message
        public static ChatMessage CreateUserChatMessage(ChatMessageContent message)
        {
            return new ChatMessage
            {
                Role = ChatMessageRole.User,
                Content =  message
            };
        }

        public static ChatMessage CreateUserChatMessage(string message)
        {
            return new ChatMessage
            {
                Role = ChatMessageRole.User,
                Content = ChatMessageContent.CreateTextMessage(message)
            };
        }
        #endregion

        #region Create Assistant Chat Message
        public static ChatMessage CreateAssistantChatMessage(ChatMessageContent message)
        {
            return new ChatMessage
            {
                Role = ChatMessageRole.Assistant,
                Content = message
            };
        }

        public static ChatMessage CreateAssistantChatMessage(string message)
        {
            return new ChatMessage
            {
                Role = ChatMessageRole.Assistant,
                Content = ChatMessageContent.CreateTextMessage(message)
            };
        }
        #endregion
    }
}
