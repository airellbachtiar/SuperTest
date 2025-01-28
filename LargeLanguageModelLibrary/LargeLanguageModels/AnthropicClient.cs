using LargeLanguageModelLibrary.Enums;
using LargeLanguageModelLibrary.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LargeLanguageModelLibrary.LargeLanguageModels
{
    public class AnthropicResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("content")]
        public List<AnthropicContent> Contents { get; set; }
        [JsonPropertyName("usage")]
        public AnthropicUsage Usage { get; set; }
    }

    public class AnthropicContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class AnthropicUsage 
    {
        [JsonPropertyName("input_tokens")]
        public int InputTokens { get; set; }
        [JsonPropertyName("output_tokens")]
        public int OutputTokens { get; set; }
    }

    public class AnthropicClient
    {
        public string ApiKey { get; init; } = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY", EnvironmentVariableTarget.Machine) ?? "";
        readonly HttpClient httpClient;

        public AnthropicClient(string apiKey) : this(new HttpClientHandler(), true)
        {
            ApiKey = apiKey;
        }

        public AnthropicClient() : this(new HttpClientHandler(), true)
        {
            ApiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY", EnvironmentVariableTarget.Machine) ?? "";
        }

        public AnthropicClient(HttpMessageHandler handler, bool disposeHandler)
        {
            httpClient = new HttpClient(handler, disposeHandler);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        }

        public async Task<MessageResponse> CompleteChatAsync(MessageRequest request, bool debugMode = false, CancellationToken cancellationToken = default)
        {
            if (debugMode)
            {
                MessageResponse response = new();
                MessageRequest messageRequest = new()
                {
                    Model = request.Model,
                    MaxTokens = request.MaxTokens,
                    Temperature = request.Temperature,
                    TopP = request.TopP,
                    TopK = request.TopK
                };

                for (int i = 0; i < request.Messages.Count; i++)
                {
                    messageRequest.Messages.Add(request.Messages[i]);
                    HttpResponseMessage httpResponseMessage = await SendRequestAsync(request, cancellationToken);
                    var anthropicResponse = JsonSerializer.Deserialize<AnthropicResponse>(await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken));
                    MessageResponseContent messageResponse = new MessageResponseContent
                    {
                        Text = anthropicResponse.Contents[0].Text,
                        TokenUsage = new TokenUsage
                        {
                            InputTokenCount = anthropicResponse.Usage.InputTokens,
                            OutputTokenCount = anthropicResponse.Usage.OutputTokens,
                            TotalTokenCount = anthropicResponse.Usage.InputTokens + anthropicResponse.Usage.OutputTokens
                        },
                        CreatedAt = DateTime.Now,
                    };

                    response.Id = anthropicResponse.Id;
                    response.ModelName = anthropicResponse.Model;
                    response.Messages.Add(messageResponse);

                    messageRequest.Messages.Add(new ChatMessage
                    {
                        Role = ChatMessageRole.Assistant,
                        Content = ChatMessageContent.CreateTextMessage(messageResponse.Text)
                    });
                }

                return response;
            }
            else
            {
                HttpResponseMessage httpResponseMessage = await SendRequestAsync(request, cancellationToken);
                var anthropicResponse = JsonSerializer.Deserialize<AnthropicResponse>(await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken));

                return new MessageResponse
                {
                    Id = anthropicResponse.Id,
                    ModelName = anthropicResponse.Model,
                    Messages = anthropicResponse.Contents.Select(content => new MessageResponseContent
                    {
                        Text = content.Text,
                        TokenUsage = new TokenUsage
                        {
                            InputTokenCount = anthropicResponse.Usage.InputTokens,
                            OutputTokenCount = anthropicResponse.Usage.OutputTokens,
                            TotalTokenCount = anthropicResponse.Usage.InputTokens + anthropicResponse.Usage.OutputTokens
                        },
                        CreatedAt = DateTime.Now,
                    }).ToList()
                };
            }
        }

        public async Task<HttpResponseMessage> SendRequestAsync(MessageRequest request, CancellationToken cancellationToken = default)
        {
            string apiUrl = "https://api.anthropic.com/v1/messages";

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("x-api-key", ApiKey);
            httpClient.DefaultRequestHeaders.Add("Anthropic-Version", "2023-06-01");

            var payload = CreatePayLoadRequest(request);
            var test = JsonSerializer.Serialize(payload);
            var jsonContent = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
            );

            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, jsonContent, cancellationToken);
            response.EnsureSuccessStatusCode();

            return response;
        }

        private object CreatePayLoadRequest(MessageRequest request)
        {
            var payload = new Dictionary<string, object>
            {
                { "model", request.Model },
                { "messages", request.Messages.Select(m => new
                    {
                        role = m.Role.ToString().ToLower(),
                        content = CreateContentPayload(m.Content)
                    }).ToArray() },
                { "max_tokens", request.MaxTokens ?? 8192 }
            };

            if (request.Temperature != null)
            {
                payload.Add("temperature", request.Temperature);
            }

            if (request.TopP != null)
            {
                payload.Add("top_p", request.TopP);
            }

            if (request.TopK != null)
            {
                payload.Add("top_k", request.TopK);
            }

            return payload;
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
                    type = "image",
                    source = new
                    {
                        type = "base64",
                        media_type = chatMessage.ImageUri.ImageBytesMediaType,
                        data = Convert.ToBase64String(chatMessage.ImageUri.ImageBytes.ToArray())
                    }
                });
            }

            return contents;
        }
    }
}
