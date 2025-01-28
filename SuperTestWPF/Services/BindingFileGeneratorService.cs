using Microsoft.Extensions.Logging;
using SuperTestLibrary;
using SuperTestWPF.Models;
using SuperTestWPF.Retry;
using System.Collections.ObjectModel;

namespace SuperTestWPF.Services
{
    public class BindingFileGeneratorService : GeneratorBaseService, IBindingFileGeneratorService
    {
        private readonly ISuperTestController _controller;
        private readonly ILogger<BindingFileGeneratorService> _logger;
        private readonly IRetryService _retry;

        public BindingFileGeneratorService(ISuperTestController controller, ILogger<BindingFileGeneratorService> logger, IRetryService retry) : base(controller, logger)
        {
            _controller = controller;
            _logger = logger;
            _retry = retry;
        }

        public async Task<SpecFlowBindingFileResponse> GenerateBindingFilesAsync(string selectedLlmString, FileInformation featureFile, ObservableCollection<FileInformation> additionalCode, CancellationToken cancellationToken = default)
        {
            try
            {
                SetLlm(selectedLlmString);

                List<SpecFlowBindingFileModel> bindingFiles = [];
                List<PromptHistory> promptHistories = [];

                _logger.LogInformation("Generating binding file...");
                var generatedBindingFile = await _retry.DoAsync(
                    () => _controller.GenerateSpecFlowBindingFileAsync(
                        featureFile.Value!,
                        additionalCode.ToDictionary(f => f.Path!, f => f.Value!),
                        cancellationToken),
                    TimeSpan.FromSeconds(1));

                foreach (var bindingFile in generatedBindingFile.BindingFiles)
                {
                    bindingFiles.Add(new SpecFlowBindingFileModel(bindingFile.Key, bindingFile.Value));
                }

                foreach (var prompt in generatedBindingFile.Prompts)
                {
                    promptHistories.Add(new PromptHistory(DateTime.Now, "Generate Binding File", selectedLlmString, prompt));
                }

                for (int i = 0; i < generatedBindingFile.RawResponse.Count && i < promptHistories.Count; i++)
                {
                    promptHistories[i].RawResponse = generatedBindingFile.RawResponse[i];
                }

                return new(bindingFiles, promptHistories);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation was cancelled.");
                throw;
            }
            catch (Exception)
            {
                _logger.LogError($"Exception while generating binding file for {featureFile.Path} using {_controller.SelectedLLM.ToString()}");
                throw;
            }
        }
    }
}
