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

        public async Task<string> GenerateBindingFilesAsync(string selectedLlmString, FileInformation featureFile, ObservableCollection<FileInformation> additionalCode)
        {
            try
            {
                SetLlm(selectedLlmString);

                _logger.LogInformation("Generating binding file...");
                var generatedBindingFile = await _retry.DoAsync(
                    () => _controller.GenerateSpecFlowBindingFileAsync(
                        featureFile.Value!,
                        additionalCode.ToDictionary(f => f.Path!, f => f.Value!)),
                    TimeSpan.FromSeconds(1));
                return generatedBindingFile.BindingFiles.First().Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception while generating binding file for {featureFile.Path} using {_controller.SelectedLLM}");
                throw;
            }
        }
    }
}
