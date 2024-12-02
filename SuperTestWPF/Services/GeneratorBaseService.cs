using LlmLibrary.Models;
using Microsoft.Extensions.Logging;
using SuperTestLibrary;

namespace SuperTestWPF.Services
{
    public class GeneratorBaseService
    {
        private readonly GPT_4o _gpt_4o = new();
        private readonly Claude_3_5_Sonnet _claude_3_5_Sonnet = new();
        private readonly Gemini_1_5 _gemini_1_5 = new();

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
                case GPT_4o.ModelName:
                    _controller.SelectedLLM = _gpt_4o;
                    break;
                case Claude_3_5_Sonnet.ModelName:
                    _controller.SelectedLLM = _claude_3_5_Sonnet;
                    break;
                case Gemini_1_5.ModelName:
                    _controller.SelectedLLM = _gemini_1_5;
                    break;
            }

            _logger.LogInformation($"Selected LLM: {selectedLlmString}");
        }
    }
}
