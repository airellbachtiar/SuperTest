using Microsoft.Extensions.Logging;
using SuperTestLibrary;
using SuperTestWPF.Helper;
using SuperTestWPF.Models;
using System.Collections.ObjectModel;

namespace SuperTestWPF.Services
{
    public class BindingFileGeneratorService : GeneratorBaseService, IBindingFileGeneratorService
    {
        private readonly ISuperTestController _controller;
        private readonly ILogger<BindingFileGeneratorService> _logger;

        public BindingFileGeneratorService(ISuperTestController controller, ILogger<BindingFileGeneratorService> logger) : base(controller, logger)
        {
            _controller = controller;
            _logger = logger;
        }

        public async Task<string> GenerateBindingFilesAsync(string selectedLlmString, FileInformation featureFile, ObservableCollection<FileInformation> additionalCode)
        {
            try
            {
                SetLlm(selectedLlmString);

                _logger.LogInformation("Generating binding file...");
                var generatedBindingFile = await Retry.DoAsync(() => _controller.GenerateSpecFlowBindingFileAsync(featureFile.Value!, additionalCode.ToDictionary(f => f.Value!, f => f.Path!)), TimeSpan.FromSeconds(1));
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
