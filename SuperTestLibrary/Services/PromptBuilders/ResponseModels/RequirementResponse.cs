﻿namespace SuperTestLibrary.Services.PromptBuilders.ResponseModels
{
    public class RequirementResponse
    {
        public string Requirement { get; set; } = string.Empty;
        public List<string> Prompts { get; set; } = [];
    }
}
