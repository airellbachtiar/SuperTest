using LlmLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SuperTestLibrary.Logger;
using SuperTestWPF.Logger;
using SuperTestWPF.Models;
using SuperTestWPF.Services;
using SuperTestWPF.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace SuperTestWPF.ViewModels
{
    public class RequirementGeneratorViewModel : ViewModelBase
    {
        public string Title { get; set; } = "Requirement Generator";

        private readonly ObservableCollection<string> _llmList = new([GPT_4o.ModelName, Claude_3_5_Sonnet.ModelName]);
        private readonly ILogger<MainWindowViewModel> _logger;

        private string _selectedLLM = GPT_4o.ModelName;
        private string _savePath = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
        private string _generatedRequirement = string.Empty;
        private ObservableCollection<FileInformation> _uploadedTestFiles = [];
        private ObservableCollection<RequirementModel> _generatedRequirements = [];
        private FileInformation? _selectedTestFile = null;
        private CancellationTokenSource? _cancellationTokenSource;
        
        private readonly IFileService _fileService;
        private readonly IRequirementGeneratorService _requirementGeneratorService;
        private readonly IPromptVerboseService _promptVerboseService;
        private readonly IReqIFConverterService _reqIFConverterService;

        public ObservableCollection<LogEntry> LogMessages { get; } = [];

        public RequirementGeneratorViewModel(IServiceProvider serviceProvider)
        {
            UploadTestFilesCommand = new RelayCommand(UploadTestFiles);
            GenerateRequirementCommand = new AsyncCommand(GenerateRequirement);
            SaveRequirementFilesCommand = new RelayCommand(SaveRequirementFiles);
            SelectSaveLocationCommand = new RelayCommand(SelectSaveLocation);

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddProvider(new ListBoxLoggerProvider(LogMessages));

            _logger = serviceProvider.GetRequiredService<ILogger<MainWindowViewModel>>();

            _fileService = serviceProvider.GetRequiredService<IFileService>();
            _requirementGeneratorService = serviceProvider.GetRequiredService<IRequirementGeneratorService>();
            _promptVerboseService = serviceProvider.GetRequiredService<IPromptVerboseService>();
            _reqIFConverterService = serviceProvider.GetRequiredService<IReqIFConverterService>();
        }

        public ObservableCollection<string> LLMList
        {
            get => _llmList;
        }

        public string SelectedLLM
        {
            get => _selectedLLM;
            set => SetProperty(ref _selectedLLM, value);
        }

        public string SavePath
        {
            get => _savePath;
            set => SetProperty(ref _savePath, value);
        }

        public ObservableCollection<FileInformation> UploadedTestFiles
        {
            get => _uploadedTestFiles;
            set => SetProperty(ref _uploadedTestFiles, value);
        }
        public FileInformation? SelectedTestFile
        {
            get => _selectedTestFile;
            set => SetProperty(ref _selectedTestFile, value);
        }
        public ObservableCollection<RequirementModel> GeneratedRequirements
        {
            get => _generatedRequirements;
            set => SetProperty(ref _generatedRequirements, value);
        }
        public string GeneratedRequirement
        {
            get => _generatedRequirement;
            set => SetProperty(ref _generatedRequirement, value);
        }

        public ICommand GenerateRequirementCommand { get; }
        public ICommand UploadTestFilesCommand { get; }
        public ICommand SaveRequirementFilesCommand { get; }
        public ICommand SelectSaveLocationCommand { get; }

        public void OnTestFileSelected(object selectedItem)
        {
            if (selectedItem is FileInformation testFile)
            {
                SelectedTestFile = testFile;
            }
        }

        private async Task GenerateRequirement()
        {
            try
            {
                if (UploadedTestFiles.Count == 0)
                {
                    _logger.LogWarning("No test files uploaded.");
                    _logger.LogError("Failed to generate requirement.");
                    return;
                }

                var response = await _requirementGeneratorService.GenerateRequirementAsync(SelectedLLM, UploadedTestFiles.ToDictionary(f => f.Path!, f => f.Value!), "", CreateNewCancellationToken());

                GeneratedRequirements = new (response.Requirements);
                foreach (var prompt in response.Prompts)
                {
                    _promptVerboseService.AddPrompt(prompt);
                }
            }
            catch { return; }
        }

        private Dictionary<string, string> GetFilesFromFolder()
        {
            var files = _fileService.OpenFilesDialog("All Files (*.*)|*.*");
            return files.ToDictionary(f => Path.GetFullPath(f), f => _fileService.GetFileContent(f));
        }

        private void UploadTestFiles()
        {
            var files = GetFilesFromFolder();
            foreach (var file in files)
            {
                UploadedTestFiles.Add(new FileInformation(file.Key, file.Value));
            }
        }

        private void SelectSaveLocation()
        {
            SavePath = _fileService.SelectFolderLocation(SavePath);
        }

        private void SaveRequirementFiles()
        {
            if (GeneratedRequirements.Count == 0)
            {
                _logger.LogWarning("No requirements generated.");
                _logger.LogError("Failed to save requirements.");
                return;
            }

            var reqIf = _reqIFConverterService.ConvertRequirementToReqIfAsync(GeneratedRequirements);

            _fileService.SaveFile(Path.Combine(SavePath, "requirements.reqif"), reqIf);

            _logger.LogInformation("Requirements saved.");
        }

        private CancellationToken CreateNewCancellationToken()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            return _cancellationTokenSource.Token;
        }
    }
}
