﻿using DotNetEnv;
using GenerativeAI.Models;
using GenerativeAI.Types;

namespace SuperTestLibrary.LLMs
{
    public class Gemini_1_5 : ILargeLanguageModel
    {
        private static readonly GenerativeModel _gemini;

        public const string ModelName = "Gemini 1.5";

        public string Id => ModelName;

        static Gemini_1_5()
        {
            Env.Load();
            string? ApiKey = Env.GetString("GEMINI_API_KEY") ?? throw new InvalidOperationException("GEMINI_API_KEY is not set.");

            _gemini = new GenerativeModel(ApiKey);
            ApiKey = null;
        }

        public async Task<string> Call(IEnumerable<string> messages)
        {
            var chat = _gemini.StartChat(new StartChatParams());

            string response = string.Empty;

            foreach (var prompt in messages)
            {
                response = await chat.SendMessageAsync(prompt);
            }

            return response;
        }
    }
}
