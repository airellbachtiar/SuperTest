﻿using SuperTestLibrary.Services;

namespace SuperTestLibrary
{
    public interface ISuperTestController
    {
        Task<string> GenerateSpecFlowFeatureFileAsync(string requirements);

        Task<IEnumerable<string>> GetAllReqIFFilesAsync();
        void SetLLM(ILLM llm);
    }
}
