using LlmLibrary.Models;
using SuperTestWPF.Helper;
using SuperTestWPF.Models;
using SuperTestWPF.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SuperTestWPF.Logger;
using SuperTestWPF.Services;

namespace SuperTestWPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const string ReqIFFileFilter = "ReqIF (*.reqif)|*.reqif|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
        private const string FeatureFileFilter = "Feature File (*.feature)|*.feature|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

        private string _chosenFile = string.Empty;
        private string _selectedLLM = GPT_4o.ModelName;
        private readonly ObservableCollection<string> _llmList = new([GPT_4o.ModelName, Claude_3_5_Sonnet.ModelName, Gemini_1_5.ModelName]);
        private ObservableCollection<string?> _onLoadedRequirementTitles = [];
        private string _savePath = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
        private ScenarioModel _selectedScenario = new();

        //SpecFlowFeatureFileModel
        private ObservableCollection<SpecFlowFeatureFileModel> _specFlowFeatureFiles = [];
        private SpecFlowFeatureFileModel _selectedSpecFlowFeatureFile = new();
        private string _evaluationSummary = string.Empty;
        private ObservableCollection<string> _evaluationScoreDetails = [];

        //Switch score details display
        private bool _isDisplayingFeatureFileScore = true;
        private bool _isFeatureFileContentVisible = false;

        //Binding fields
        private ObservableCollection<FileInformation> _uploadedFiles = [];
        private string _generatedBindingFile = string.Empty;
        private FileInformation? _uploadedFeatureFile = null;

        // Logger
        private readonly ILogger<MainWindowViewModel> _logger;

        //Services
        private readonly IGetReqIfService _getReqIfService;
        private readonly IFeatureFileGeneratorService _featureFileService;
        private readonly IFileService _fileService;
        private readonly IEvaluateFeatureFileService _evaluateFeatureFileService;
        private readonly IBindingFileGeneratorService _bindingFileGeneratorService;

        public ObservableCollection<LogEntry> LogMessages { get; } = [];

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            UploadReqIFCommand = new RelayCommand(UploadReqIF);
            GenerateAndEvaluateSpecFlowFeatureFileCommand = new AsyncCommand(GenerateAndEvaluateSpecFlowFeatureFile);
            DisplayFeatureFileScoreCommand = new RelayCommand(DisplayFeatureFileScore);
            DisplayScenarioScoreCommand = new RelayCommand(DisplayScenarioScore);
            SelectSaveLocationCommand = new RelayCommand(SelectSaveLocation);
            SaveFeatureFilesCommand = new RelayCommand(SaveFeatureFiles);
            SwitchFeatureFileViewCommand = new RelayCommand(SwitchFeatureFileView);

            // Binding Generator
            GenerateBindingsCommand = new AsyncCommand(GenerateBindings);
            UploadFilesCommand = new RelayCommand(UploadFiles);
            UploadFeatureFileCommand = new RelayCommand(UploadFeatureFile);
            ClearAllUploadedFilesCommand = new RelayCommand(ClearAllUpLoadedFiles);

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddProvider(new ListBoxLoggerProvider(LogMessages));

            _logger = serviceProvider.GetRequiredService<ILogger<MainWindowViewModel>>();

            //Services
            _getReqIfService = serviceProvider.GetRequiredService<IGetReqIfService>();
            _featureFileService = serviceProvider.GetRequiredService<IFeatureFileGeneratorService>();
            _fileService = serviceProvider.GetRequiredService<IFileService>();
            _evaluateFeatureFileService = serviceProvider.GetRequiredService<IEvaluateFeatureFileService>();
            _bindingFileGeneratorService = serviceProvider.GetRequiredService<IBindingFileGeneratorService>();
            _ = InitializeReqIFs();
        }

        public string ChosenFile
        {
            get => _chosenFile;
            set => SetProperty(ref _chosenFile, value);
        }

        public string SavePath
        {
            get => _savePath;
            set => SetProperty(ref _savePath, value);
        }

        public string EvaluationSummary
        {
            get => _evaluationSummary;
            set => SetProperty(ref _evaluationSummary, value);
        }

        public bool IsFeatureFileContentVisible
        {
            get => _isFeatureFileContentVisible;
            set => SetProperty(ref _isFeatureFileContentVisible, value);
        }

        public ObservableCollection<string?> OnLoadedRequirementTitles
        {
            get => _onLoadedRequirementTitles;
            set => SetProperty(ref _onLoadedRequirementTitles, value);
        }

        public ObservableCollection<string> EvaluationScoreDetails
        {
            get => _evaluationScoreDetails;
            set => SetProperty(ref _evaluationScoreDetails, value);
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

        public ObservableCollection<SpecFlowFeatureFileModel> SpecFlowFeatureFiles
        {
            get => _specFlowFeatureFiles;
            set => SetProperty(ref _specFlowFeatureFiles, value);
        }

        public SpecFlowFeatureFileModel SelectedSpecFlowFeatureFile
        {
            get => _selectedSpecFlowFeatureFile;
            set
            {
                if (SetProperty(ref _selectedSpecFlowFeatureFile, value))
                {
                    UpdateFeatureFileScores();
                }
            }
        }

        public ScenarioModel SelectedScenario
        {
            get => _selectedScenario;
            set
            {
                if (_selectedScenario != value)
                {
                    if (_selectedScenario != null)
                    {
                        _selectedScenario.IsSelected = false;
                    }
                    _selectedScenario = value;
                    if (_selectedScenario != null)
                    {
                        _selectedScenario.IsSelected = true;
                    }
                    UpdateFeatureFileScores();
                    OnPropertyChanged(nameof(SelectedScenario));
                }
            }
        }

        // Binding properties
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

        public FileInformation? UploadedFeatureFile
        {
            get => _uploadedFeatureFile;
            set => SetProperty(ref _uploadedFeatureFile, value);
        }

        public ICommand UploadReqIFCommand { get; }
        public ICommand GenerateAndEvaluateSpecFlowFeatureFileCommand { get; }
        public ICommand DisplayFeatureFileScoreCommand { get; }
        public ICommand DisplayScenarioScoreCommand { get; }
        public ICommand SelectSaveLocationCommand { get; }
        public ICommand SaveFeatureFilesCommand { get; }
        public ICommand SwitchFeatureFileViewCommand { get; }
        // Binding commands
        public ICommand GenerateBindingsCommand { get; }
        public ICommand UploadFilesCommand { get; }
        public ICommand UploadFeatureFileCommand { get; }
        public ICommand ClearAllUploadedFilesCommand { get; }

        private async Task InitializeReqIFs()
        {
            IEnumerable<string> AllReqIfFiles = await _getReqIfService.GetAll();
            OnLoadedRequirementTitles = new ObservableCollection<string?>(AllReqIfFiles);
        }

        public void OnTreeViewItemSelected(object selectedItem)
        {
            if (selectedItem is FileInformation reqIFValueAndPath)
            {
                ChosenFile = reqIFValueAndPath.Path!;
            }
        }

        private void UploadReqIF()
        {
            _logger.LogInformation("Uploading ReqIF...");
            string filepath = _fileService.OpenFileDialog(ReqIFFileFilter);
            if (string.IsNullOrEmpty(filepath)) return;

            ChosenFile = filepath;
            _logger.LogInformation("ReqIF uploaded.");
        }

        private async Task GenerateAndEvaluateSpecFlowFeatureFile()
        {
            SelectedSpecFlowFeatureFile = new();
            SpecFlowFeatureFiles.Clear();

            _logger.LogInformation("Generating SpecFlow feature file...");

            if (string.IsNullOrEmpty(ChosenFile))
            {
                _logger.LogWarning("No file chosen.");
                _logger.LogError("Failed to generate SpecFlow feature file.");
                return;
            }

            string requirements = _fileService.GetFileContent(ChosenFile);

            try
            {
                SpecFlowFeatureFiles = new ObservableCollection<SpecFlowFeatureFileModel>(await _featureFileService.GenerateSpecFlowFeatureFilesAsync(SelectedLLM, requirements));
                SelectedSpecFlowFeatureFile = SpecFlowFeatureFiles.FirstOrDefault() ?? new();
                SelectedScenario = SelectedSpecFlowFeatureFile.Scenarios.FirstOrDefault() ?? new();
                await EvaluateSpecFlowFeatureFile(requirements);
            }
            catch { return; }
        }

        private async Task EvaluateSpecFlowFeatureFile(string requirements)
        {
            if (!SpecFlowFeatureFiles.Any())
            {
                _logger.LogWarning("No feature file to evaluate.");
                return;
            }

            foreach (var featureFile in SpecFlowFeatureFiles)
            {
                foreach (var llm in new[] { GPT_4o.ModelName, Claude_3_5_Sonnet.ModelName })
                {
                    // Evaluate feature file
                    _logger.LogInformation($"Evaluating {featureFile.FeatureFileName} feature file using GPT-4o...");
                    await _evaluateFeatureFileService.EvaluateFeatureFileAsync(llm, featureFile, requirements);
                    //Evaluate scenario
                    _logger.LogInformation($"Evaluating {featureFile.FeatureFileName} scenario using GPT-4o...");
                    await _evaluateFeatureFileService.EvaluateSpecFlowScenarioAsync(llm, featureFile, requirements);
                }
            }
            _logger.LogInformation("Finished evaluating feature file!");
        }

        private void DisplayFeatureFileScore()
        {
            _isDisplayingFeatureFileScore = true;
            UpdateFeatureFileScores();
        }

        private void DisplayScenarioScore()
        {
            _isDisplayingFeatureFileScore = false;
            UpdateFeatureFileScores();
        }

        private void UpdateFeatureFileScores()
        {
            if (SelectedSpecFlowFeatureFile == null) return;
            if(_isDisplayingFeatureFileScore)
            {
                EvaluationSummary = SelectedSpecFlowFeatureFile.FeatureFileEvaluationSummary ?? string.Empty;
                EvaluationScoreDetails = SelectedSpecFlowFeatureFile.FeatureFileEvaluationScoreDetails ?? [];
            }
            else
            {
                if (SelectedScenario == null) return;
                EvaluationSummary = SelectedScenario.ScenarioEvaluationSummary ?? string.Empty;
                EvaluationScoreDetails = SelectedScenario.ScenarioEvaluationScoreDetails ?? [];
            }
        }

        private void SelectSaveLocation()
        {
            SavePath = _fileService.SelectFolderLocation(SavePath);
        }

        private void SaveFeatureFiles()
        {
            foreach (var featureFile in SpecFlowFeatureFiles)
            {
                string savePath = $"{SavePath}/{featureFile.FeatureFileName}";
                string featureFileContent = GetReviewedFeatureFile.GetAcceptedScenarios(featureFile);

                File.WriteAllText(savePath, featureFileContent);
                _logger.LogInformation($"Feature file saved to \"{savePath}\".");
            }
        }

        private void SwitchFeatureFileView()
        {
            IsFeatureFileContentVisible = !IsFeatureFileContentVisible;
        }

        // Binding Generator

        private async Task GenerateBindings()
        {
            GeneratedBindingFile = string.Empty;
            if (UploadedFiles.Count == 0)
            {
                _logger.LogWarning("No files uploaded.");
                _logger.LogError("Failed to generate binding file.");
                return;
            }

            if (UploadedFeatureFile == null)
            {
                _logger.LogWarning("No feature file uploaded.");
                _logger.LogError("Failed to generate binding file.");
                return;
            }

            GeneratedBindingFile = await _bindingFileGeneratorService.GenerateBindingFilesAsync(SelectedLLM, UploadedFeatureFile, UploadedFiles);
            _logger.LogInformation("Binding file generated.");
        }

        private void UploadFiles()
        {
            var files = GetFilesFromFolder();
            foreach (var file in files)
            {
                UploadedFiles.Add(new FileInformation(file.Key, file.Value));
            }
        }

        private void UploadFeatureFile()
        {
            _ = GetFeatureFileFromFolder();
            _logger.LogInformation("Feature file uploaded.");
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
    }
}
