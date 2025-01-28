using LargeLanguageModelLibrary.Enums;
using LargeLanguageModelLibrary.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LargeLanguageModelLibrary.LargeLanguageModels
{
    public class OpenAIResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("object")]
        public string Object { get; set; }
        [JsonPropertyName("created")]
        public long Created { get; set; }
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("choices")]
        public Choice[] Choices { get; set; }
        [JsonPropertyName("usage")]
        public OpenAIUsage Usage { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }
        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class OpenAIUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }
        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

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
                    TopP = request.TopP
                };

                for (int i = 0; i < request.Messages.Count; i++)
                {
                    messageRequest.Messages.Add(request.Messages[i]);
                    HttpResponseMessage httpResponseMessage = await SendRequestAsync(messageRequest, cancellationToken);
                    OpenAIResponse openAIResponse = JsonSerializer.Deserialize<OpenAIResponse>(await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken));
                    MessageResponseContent messageResponse = new()
                    {
                        Text = openAIResponse.Choices[0].Message.Content,
                        TokenUsage = new TokenUsage
                        {
                            InputTokenCount = openAIResponse.Usage.PromptTokens,
                            OutputTokenCount = openAIResponse.Usage.CompletionTokens,
                            TotalTokenCount = openAIResponse.Usage.TotalTokens
                        },
                        CreatedAt = DateTimeOffset.FromUnixTimeSeconds(openAIResponse.Created).DateTime
                    };

                    response.Id = openAIResponse.Id;
                    response.ModelName = openAIResponse.Model;
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
                OpenAIResponse openAIResponse = JsonSerializer.Deserialize<OpenAIResponse>(await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken));

                return new MessageResponse
                {
                    Id = openAIResponse.Id,
                    ModelName = openAIResponse.Model,
                    Messages = openAIResponse.Choices.Select(c => new MessageResponseContent
                    {
                        Text = c.Message.Content,
                        TokenUsage = new TokenUsage
                        {
                            InputTokenCount = openAIResponse.Usage.PromptTokens,
                            OutputTokenCount = openAIResponse.Usage.CompletionTokens,
                            TotalTokenCount = openAIResponse.Usage.TotalTokens
                        },
                        CreatedAt = DateTimeOffset.FromUnixTimeSeconds(openAIResponse.Created).DateTime
                    }).ToList()
                };
            }
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
