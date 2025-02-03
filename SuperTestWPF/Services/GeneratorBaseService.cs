using LargeLanguageModelLibrary.Enums;
using Microsoft.Extensions.Logging;
using SuperTestLibrary;

namespace SuperTestWPF.Services
{
    public class GeneratorBaseService
    {
        private readonly ISuperTestController _controller;
        private readonly ILogger<GeneratorBaseService> _logger;

        public GeneratorBaseService(ISuperTestController controller, ILogger<GeneratorBaseService> logger)
        {
            _controller = controller;
            _logger = logger;
        }

        public void SetLlm(string selectedLlmString)
        {
            switch (selectedLlmString)
            {
                case string gpt4o when gpt4o == ModelName.GPT4o.GetDescription():
                    _controller.SelectedLLM = ModelName.GPT4o;
                    break;
                case string claude when claude == ModelName.Claude35Sonnet.GetDescription():
                    _controller.SelectedLLM = ModelName.Claude35Sonnet;
                    break;
                case string gemini when gemini == ModelName.Gemini15.GetDescription():
                    _controller.SelectedLLM = ModelName.Gemini15;
                    break;
                case string deepSeek when deepSeek == ModelName.DeepSeekR18B.GetDescription():
                    _controller.SelectedLLM = ModelName.DeepSeekR18B;
                    break;
                default:
                    _logger.LogWarning($"Invalid LLM selection: {selectedLlmString}");
                    return;
            }

            _logger.LogInformation($"Selected LLM: {selectedLlmString}");
        }
    }
}
