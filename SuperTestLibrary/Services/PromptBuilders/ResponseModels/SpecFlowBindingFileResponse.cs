﻿namespace SuperTestLibrary.Services.PromptBuilders.ResponseModels
{
    public class SpecFlowBindingFileResponse
    {
        // Key: BindingFileName, Value: BindingFileContent
        public Dictionary<string, string> BindingFiles { get; init; } = [];
        public List<string> Prompts { get; set; } = [];
    }
}
