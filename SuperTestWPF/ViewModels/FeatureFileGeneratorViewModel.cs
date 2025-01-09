using LlmLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SuperTestLibrary.Logger;
using SuperTestWPF.Helper;
using SuperTestWPF.Logger;
using SuperTestWPF.Models;
using SuperTestWPF.Services;
using SuperTestWPF.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SuperTestWPF.ViewModels
{
    public class FeatureFileGeneratorViewModel : ViewModelBase
    {
        public string Title { get; set; } = "Feature File Generator";

        private const string ReqIFFileFilter = "ReqIF (*.reqif)|*.reqif|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
        private readonly ObservableCollection<string> _llmList = new([GPT_4o.ModelName, Claude_3_5_Sonnet.ModelName]);
        private readonly ILogger<FeatureFileGeneratorViewModel> _logger;

        private string _chosenFile = string.Empty;
        private string _selectedLLM = GPT_4o.ModelName;
        private string _savePath = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
        private string _evaluationSummary = string.Empty;
        private ObservableCollection<string?> _onLoadedRequirementTitles = [];
        private ObservableCollection<string> _evaluationScoreDetails = [];
        private ObservableCollection<SpecFlowFeatureFileModel> _specFlowFeatureFiles = [];
        private ScenarioModel? _selectedScenario = null;
        private SpecFlowFeatureFileModel _selectedSpecFlowFeatureFile = new();
        private CancellationTokenSource? _cancellationTokenSource;

        //Switch score details in FeatureFileGeneratorView
        private bool _isDisplayingFeatureFileScore = true;
        private bool _isFeatureFileContentVisible = false;

        private readonly IServiceProvider _serviceProvider;
        private readonly IGetReqIfService _getReqIfService;
        private readonly IFeatureFileGeneratorService _featureFileService;
        private readonly IFileService _fileService;
        private readonly IEvaluateFeatureFileService _evaluateFeatureFileService;
        private readonly IPromptVerboseService _promptVerboseService;

        public ObservableCollection<LogEntry> LogMessages { get; } = [];
        
        public FeatureFileGeneratorViewModel(IServiceProvider serviceProvider)
        {
            UploadReqIFCommand = new RelayCommand(UploadReqIF);
            GenerateAndEvaluateSpecFlowFeatureFileCommand = new AsyncCommand(GenerateAndEvaluateSpecFlowFeatureFile);
            DisplayFeatureFileScoreCommand = new RelayCommand(DisplayFeatureFileScore);
            DisplayScenarioScoreCommand = new RelayCommand(DisplayScenarioScore);
            SelectSaveLocationCommand = new RelayCommand(SelectSaveLocation);
            SaveFeatureFilesCommand = new RelayCommand(SaveFeatureFiles);
            SwitchFeatureFileViewCommand = new RelayCommand(SwitchFeatureFileView);
            OpenSettingsCommand = new AsyncCommand(OpenSettings);

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddProvider(new ListBoxLoggerProvider(LogMessages));

            _logger = serviceProvider.GetRequiredService<ILogger<FeatureFileGeneratorViewModel>>();

            //Services
            _getReqIfService = serviceProvider.GetRequiredService<IGetReqIfService>();
            _featureFileService = serviceProvider.GetRequiredService<IFeatureFileGeneratorService>();
            _fileService = serviceProvider.GetRequiredService<IFileService>();
            _evaluateFeatureFileService = serviceProvider.GetRequiredService<IEvaluateFeatureFileService>();
            _promptVerboseService = serviceProvider.GetRequiredService<IPromptVerboseService>();
            _serviceProvider = serviceProvider;
            _ = InitializeReqIFs();
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

        public ScenarioModel? SelectedScenario
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

        public ICommand UploadReqIFCommand { get; }
        public ICommand GenerateAndEvaluateSpecFlowFeatureFileCommand { get; }
        public ICommand DisplayFeatureFileScoreCommand { get; }
        public ICommand DisplayScenarioScoreCommand { get; }
        public ICommand SelectSaveLocationCommand { get; }
        public ICommand SaveFeatureFilesCommand { get; }
        public ICommand SwitchFeatureFileViewCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        public async Task InitializeReqIFs()
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

        private async Task OpenSettings()
        {
            var settingsWindow = new SettingsWindow(new SettingsWindowViewModel(_serviceProvider));
            settingsWindow.ShowDialog();

            await InitializeReqIFs();
        }

        public void UploadReqIF()
        {
            _logger.LogInformation("Uploading ReqIF...");
            string filepath = _fileService.OpenFileDialog(ReqIFFileFilter);
            if (string.IsNullOrEmpty(filepath)) return;

            ChosenFile = filepath;
            _logger.LogInformation("ReqIF uploaded.");
        }

        public async Task GenerateAndEvaluateSpecFlowFeatureFile()
        {
            var cancellationToken = CreateNewCancellationToken();
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
                var specFlowFeatureFileResponse = await _featureFileService.GenerateSpecFlowFeatureFilesAsync(SelectedLLM, requirements, cancellationToken);
                SpecFlowFeatureFiles = new ObservableCollection<SpecFlowFeatureFileModel>(specFlowFeatureFileResponse.FeatureFiles);

                foreach (var prompt in specFlowFeatureFileResponse.Prompts)
                {
                    _promptVerboseService.AddPrompt(prompt);
                }

                SelectedSpecFlowFeatureFile = SpecFlowFeatureFiles.FirstOrDefault() ?? new();
                SelectedScenario = SelectedSpecFlowFeatureFile.Scenarios.FirstOrDefault() ?? new();
                await EvaluateSpecFlowFeatureFile(requirements, cancellationToken);
            }
            catch { return; }
        }

        public async Task EvaluateSpecFlowFeatureFile(string requirements, CancellationToken cancellationToken)
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
                    foreach (var prompt in await _evaluateFeatureFileService.EvaluateFeatureFileAsync(llm, featureFile, requirements, cancellationToken))
                    {
                        _promptVerboseService.AddPrompt(prompt);
                    }

                    //Evaluate scenario
                    _logger.LogInformation($"Evaluating {featureFile.FeatureFileName} scenario using GPT-4o...");
                    foreach (var prompt in await _evaluateFeatureFileService.EvaluateSpecFlowScenarioAsync(llm, featureFile, requirements, cancellationToken))
                    {
                        _promptVerboseService.AddPrompt(prompt);
                    }
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
            if (_isDisplayingFeatureFileScore)
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

                _fileService.SaveFile(savePath, featureFileContent);
            }
        }

        public void SwitchFeatureFileView()
        {
            IsFeatureFileContentVisible = !IsFeatureFileContentVisible;
        }

        private CancellationToken CreateNewCancellationToken()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            return _cancellationTokenSource.Token;
        }
    }
}
