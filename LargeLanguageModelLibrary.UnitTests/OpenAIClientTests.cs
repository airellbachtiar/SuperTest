using LargeLanguageModelLibrary.LargeLanguageModels;
using LargeLanguageModelLibrary.Models;
using System.Net;

namespace LargeLanguageModelLibrary.UnitTests
{
    public class OpenAIClientTests
    {
        [Test]
        public async Task SendRequestAsync_MultipleMessages_ShouldReturnOKAndAllResponses()
        {
            MessageRequest messageRequest = new()
            {
                Messages = 
                [
                    ChatMessage.CreateUserChatMessage("Hi"),
                ],
                Model = "gpt-4o",
                MaxTokens = 2,
            };

            OpenAIClient client = new();
            var response = await client.SendRequestAsync(messageRequest);
            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(content);
        }
    }
}