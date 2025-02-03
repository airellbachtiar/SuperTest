using LargeLanguageModelLibrary.Models;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using System;
using LargeLanguageModelLibrary.Enums;

namespace LargeLanguageModelLibrary.LargeLanguageModels
{
    public class OllamaResponse
    {
        [JsonPropertyName("model")]
        public string ModelName { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("message")]
        public OllamaMessage Message { get; set; } = new();
    }

    public class OllamaMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class OllamaClient
    {
        private readonly HttpClient _httpClient;

        public OllamaClient() : this(new HttpClientHandler(), true)
        {
        }

        public OllamaClient (HttpMessageHandler handler, bool disposeHandler)
        {
            _httpClient = new HttpClient(handler, disposeHandler);
        }

        public async Task<MessageResponse> CompleteChatAsync(MessageRequest request, bool debugMode = false, CancellationToken cancellationToken = default)
        {
            if (debugMode)
            {
                MessageResponse response = new();
                MessageRequest messageRequest = new()
                {
                    Model = request.Model
                };

                for (int i = 0; i < request.Messages.Count; i++)
                {
                    messageRequest.Messages.Add(request.Messages[i]);
                    HttpResponseMessage httpResponseMessage = await SendRequestAsync(messageRequest, cancellationToken);
                    OllamaResponse ollamaResponse = await JsonSerializer.DeserializeAsync<OllamaResponse>(await httpResponseMessage.Content.ReadAsStreamAsync(), cancellationToken: cancellationToken);
                    MessageResponseContent messageResponseContent = new()
                    {
                        Text = ollamaResponse.Message.Content,
                        CreatedAt = DateTime.Parse(ollamaResponse.CreatedAt)
                    };

                    response.ModelName = ollamaResponse.ModelName;
                    response.Messages.Add(messageResponseContent);

                    messageRequest.Messages.Add(new ChatMessage
                    {
                        Role = ChatMessageRole.Assistant,
                        Content = ChatMessageContent.CreateTextMessage(messageResponseContent.Text)
                    });
                }

                return response;
            }
            else
            {
                HttpResponseMessage response = await SendRequestAsync(request, cancellationToken);
                OllamaResponse ollamaResponse = await JsonSerializer.DeserializeAsync<OllamaResponse>(await response.Content.ReadAsStreamAsync(), cancellationToken: cancellationToken);

                return new MessageResponse
                {
                    ModelName = ollamaResponse.ModelName,
                    Messages =
                    [
                        new() {
                            Text = ollamaResponse.Message.Content,
                            CreatedAt = DateTime.Parse(ollamaResponse.CreatedAt)
                        }
                    ]
                };
            }
        }

        public async Task<HttpResponseMessage> SendRequestAsync(MessageRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                string apiUrl = "http://127.0.0.1:11434/api/chat";

                var payload = CreatePayLoadRequest(request);

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, jsonContent, cancellationToken);

                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Error occurred while sending request to Ollama API.", ex);
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
                stream = false
            };
        }

        private object CreateContentPayload(ChatMessageContent chatMessage)
        {
            if (chatMessage.Text != null)
            {
                return chatMessage.Text;
            }

            if (chatMessage.ImageUri != null)
            {
                return Convert.ToBase64String(chatMessage.ImageUri.ImageBytes.ToArray());
            }

            return null;
        }
    }
}
