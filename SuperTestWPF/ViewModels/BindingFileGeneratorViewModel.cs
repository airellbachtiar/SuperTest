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
    public class BindingFileGeneratorViewModel : ViewModelBase
    {
        public string Title { get; set; } = "Binding File Generator";

        private const string FeatureFileFilter = "Feature File (*.feature)|*.feature|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
        private readonly ObservableCollection<string> _llmList = new([GPT_4o.ModelName, Claude_3_5_Sonnet.ModelName]);
        private readonly ILogger<BindingFileGeneratorViewModel> _logger;

        private string _selectedLLM = GPT_4o.ModelName;
        private string _savePath = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
        private string _generatedBindingFile = string.Empty;
        private ObservableCollection<FileInformation> _uploadedFiles = [];
        private ObservableCollection<SpecFlowBindingFileModel> _specFlowBindingFiles = [];
        private FileInformation? _uploadedFeatureFile = null;
        private FileInformation? _selectedUploadedFile = null;
        private CancellationTokenSource? _cancellationTokenSource;
        
        private readonly IFileService _fileService;
        private readonly IBindingFileGeneratorService _bindingFileGeneratorService;
        private readonly IPromptVerboseService _promptVerboseService;

        public ObservableCollection<LogEntry> LogMessages { get; } = [];

        public BindingFileGeneratorViewModel(IServiceProvider serviceProvider)
        {
            GenerateBindingsCommand = new AsyncCommand(GenerateBindings);
            UploadFilesCommand = new RelayCommand(UploadFiles);
            UploadFeatureFileCommand = new RelayCommand(UploadFeatureFile);
            ClearAllUploadedFilesCommand = new RelayCommand(ClearAllUpLoadedFiles);
            SaveBindingFileCommand = new RelayCommand(SaveBindingFile);
            ViewFeatureFileCommand = new RelayCommand(ViewFeatureFile);
            SelectSaveLocationCommand = new RelayCommand(SelectSaveLocation);

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddProvider(new ListBoxLoggerProvider(LogMessages));

            _logger = serviceProvider.GetRequiredService<ILogger<BindingFileGeneratorViewModel>>();

            _fileService = serviceProvider.GetRequiredService<IFileService>();
            _bindingFileGeneratorService = serviceProvider.GetRequiredService<IBindingFileGeneratorService>();
            _promptVerboseService = serviceProvider.GetRequiredService<IPromptVerboseService>();
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

        public ObservableCollection<FileInformation> UploadedFiles
        {
            get => _uploadedFiles;
            set => SetProperty(ref _uploadedFiles, value);
        }

        public string GeneratedBindingFile
        {
            get => _generatedBindingFile;
            set => SetProperty(ref _generatedBindingFile, value);
        }

        public ObservableCollection<SpecFlowBindingFileModel> SpecFlowBindingFiles
        {
            get => _specFlowBindingFiles;
            set => SetProperty(ref _specFlowBindingFiles, value);
        }
        public FileInformation? UploadedFeatureFile
        {
            get => _uploadedFeatureFile;
            set => SetProperty(ref _uploadedFeatureFile, value);
        }

        public FileInformation? SelectedUploadedFile
        {
            get => _selectedUploadedFile;
            set => SetProperty(ref _selectedUploadedFile, value);
        }

        public ICommand GenerateBindingsCommand { get; }
        public ICommand UploadFilesCommand { get; }
        public ICommand UploadFeatureFileCommand { get; }
        public ICommand ClearAllUploadedFilesCommand { get; }
        public ICommand SaveBindingFileCommand { get; }
        public ICommand ViewFeatureFileCommand { get; }
        public ICommand SelectSaveLocationCommand { get; }

        public void OnTreeViewItemSelected(object selectedItem)
        {
            if (selectedItem is FileInformation uploadedFile)
            {
                SelectedUploadedFile = uploadedFile;
            }
        }

        public async Task GenerateBindings()
        {
            var cancellationToken = CreateNewCancellationToken();
            GeneratedBindingFile = string.Empty;

            if (UploadedFeatureFile == null)
            {
                _logger.LogWarning("No feature file uploaded.");
                _logger.LogError("Failed to generate binding file.");
                return;
            }

            try
            {
                var response = await _bindingFileGeneratorService.GenerateBindingFilesAsync(SelectedLLM, UploadedFeatureFile, UploadedFiles, cancellationToken);
                SpecFlowBindingFiles = new ObservableCollection<SpecFlowBindingFileModel>(response.specFlowBindingFileModels);
                GeneratedBindingFile = response.specFlowBindingFileModels.FirstOrDefault()?.BindingFileContent ?? string.Empty;

                foreach (var prompt in response.Prompts)
                {
                    _promptVerboseService.AddPrompt(prompt);
                }

                _logger.LogInformation("Binding file generated.");
            }
            catch { return; }
        }

        public void UploadFiles()
        {
            var files = GetFilesFromFolder();
            foreach (var file in files)
            {
                UploadedFiles.Add(new FileInformation(file.Key, file.Value));
            }
        }

        public void UploadFeatureFile()
        {
            _ = GetFeatureFileFromFolder();
        }

        private string GetFeatureFileFromFolder()
        {
            string filepath = _fileService.OpenFileDialog(FeatureFileFilter);
            if (string.IsNullOrEmpty(filepath)) return string.Empty;

            UploadedFeatureFile = new FileInformation(Path.GetFileName(filepath), _fileService.GetFileContent(filepath));
            _logger.LogInformation("Feature file uploaded.");

            return filepath;
        }

        private Dictionary<string, string> GetFilesFromFolder()
        {
            var files = _fileService.OpenFilesDialog("All Files (*.*)|*.*");
            return files.ToDictionary(f => Path.GetFullPath(f), f => _fileService.GetFileContent(f));
        }

        private void ClearAllUpLoadedFiles()
        {
            UploadedFiles.Clear();
            UploadedFeatureFile = null;
        }

        public void SaveBindingFile()
        {
            if (string.IsNullOrEmpty(GeneratedBindingFile))
            {
                _logger.LogWarning("No binding file generated.");
                _logger.LogError("Failed to save binding file.");
                return;
            }

            string savePath = $"{SavePath}/";
            foreach (var bindingFile in SpecFlowBindingFiles)
            {
                savePath += bindingFile.BindingFileName;
                _fileService.SaveFile(savePath, bindingFile.BindingFileContent);
            }
        }

        private CancellationToken CreateNewCancellationToken()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            return _cancellationTokenSource.Token;
        }

        private void ViewFeatureFile()
        {
            SelectedUploadedFile = UploadedFeatureFile;
        }

        private void SelectSaveLocation()
        {
            SavePath = _fileService.SelectFolderLocation(SavePath);
        }
    }
}
