using LargeLanguageModelLibrary.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LargeLanguageModelLibrary.LargeLanguageModels
{
    public class OpenAIClient
    {
        public string ApiKey { get; init; } = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Machine) ?? "";
        readonly HttpClient httpClient;

        public OpenAIClient(string apiKey) : this(new HttpClientHandler(), true)
        {
            ApiKey = apiKey;
        }

        public OpenAIClient() : this(new HttpClientHandler(), true)
        {
        }

        public OpenAIClient(HttpMessageHandler handler, bool disposeHandler)
        {
            this.httpClient = new HttpClient(handler, disposeHandler);
        }

        public async Task<HttpResponseMessage> SendRequestAsync(MessageRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                string apiUrl = "https://api.openai.com/v1/chat/completions";

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                var payload = CreatePayLoadRequest(request);

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, jsonContent, cancellationToken);

                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Error occurred while sending request to OpenAI API.", ex);
            }
        }

        private object CreatePayLoadRequest(MessageRequest request)
        {
            return new
            {
                model = request.Model,
                messages = request.Messages.Select(m => new
                {
                    role = m.Role.ToString().ToLower(),
                    content = CreateContentPayload(m.Content)
                }).ToArray(),
                max_completion_tokens = request.MaxTokens,
                temperature = request.Temperature,
                top_p = request.TopP
            };
        }

        private object CreateContentPayload(ChatMessageContent chatMessage)
        {
            List<object> contents = [];
            if (chatMessage.Text != null)
            {
                contents.Add(new
                {
                    type = "text",
                    text = chatMessage.Text
                });
            }

            if (chatMessage.ImageUri != null)
            {
                contents.Add(new
                {
                    type = "image_url",
                    image_url = new { url = chatMessage.ImageUri.Url}
                });
            }

            return contents;
        }
    }
}
