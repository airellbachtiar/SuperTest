using Microsoft.Win32;
using SuperTestLibrary;
using LlmLibrary;
using LlmLibrary.Models;
using SuperTestWPF.Helper;
using SuperTestWPF.Models;
using SuperTestWPF.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace SuperTestWPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> _statusMessages = [];
        private string _chosenFile = string.Empty;
        private string _selectedLLM = GPT_4o.ModelName;
        private readonly ISuperTestController _superTestController;
        private readonly ObservableCollection<string> _llmList = new([GPT_4o.ModelName, Claude_3_5_Sonnet.ModelName, Gemini_1_5.ModelName]);
        private ObservableCollection<string?> _onLoadedRequirementTitles = [];
        private string _savePath = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
        private ScenarioModel _selectedScenario = new();

        // LLM
        private readonly GPT_4o _gpt_4o = new();
        private readonly Claude_3_5_Sonnet _claude_3_5_Sonnet = new();
        private readonly Gemini_1_5 _gemini_1_5 = new();

        //SpecFlowFeatureFileModel
        private ObservableCollection<SpecFlowFeatureFileModel> _specFlowFeatureFiles = [];
        private SpecFlowFeatureFileModel _selectedSpecFlowFeatureFile = new();
        private string _evaluationSummary = string.Empty;
        private ObservableCollection<string> _evaluationScoreDetails = [];

        //Switch score details display
        private bool _isDisplayingFeatureFileScore = true;
        private bool _isFeatureFileContentVisible = false;

        public MainWindowViewModel(ISuperTestController superTestController)
        {
            UploadReqIFCommand = new RelayCommand(UploadReqIF);
            GenerateAndEvaluateSpecFlowFeatureFileCommand = new RelayCommand(GenerateAndEvaluateSpecFlowFeatureFile);
            DisplayFeatureFileScoreCommand = new RelayCommand(DisplayFeatureFileScore);
            DisplayScenarioScoreCommand = new RelayCommand(DisplayScenarioScore);
            SelectSaveLocationCommand = new RelayCommand(SelectSaveLocation);
            SaveFeatureFilesCommand = new RelayCommand(SaveFeatureFiles);
            SwitchFeatureFileViewCommand = new RelayCommand(SwitchFeatureFileView);

            this._superTestController = superTestController;
            _ = InitializeReqIFs();
        }

        private async Task InitializeReqIFs()
        {
            var AllReqIfFiles = await _superTestController.GetAllReqIFFilesAsync();

            OnLoadedRequirementTitles = new ObservableCollection<string?>(AllReqIfFiles);
        }

        public ObservableCollection<string> StatusMessages
        {
            get { return _statusMessages; }
            set
            {
                if (_statusMessages != value)
                {
                    _statusMessages = value;
                    OnPropertyChanged(nameof(StatusMessages));
                }
            }
        }

        public string ChosenFile
        {
            get { return _chosenFile; }
            set
            {
                if (_chosenFile != value)
                {
                    _chosenFile = value;
                    OnPropertyChanged(nameof(ChosenFile));
                }
            }
        }

        public string SavePath
        {
            get { return _savePath; }
            set
            {
                if (_savePath != value)
                {
                    _savePath = value;
                    OnPropertyChanged(nameof(SavePath));
                }
            }
        }

        public string EvaluationSummary
        {
            get { return _evaluationSummary; }
            set
            {
                if (_evaluationSummary != value)
                {
                    _evaluationSummary = value;
                    OnPropertyChanged(nameof(EvaluationSummary));
                }
            }
        }

        public bool IsFeatureFileContentVisible
        {
            get { return _isFeatureFileContentVisible; }
            set
            {
                if (_isFeatureFileContentVisible != value)
                {
                    _isFeatureFileContentVisible = value;
                    OnPropertyChanged(nameof(IsFeatureFileContentVisible));
                }
            }
        }

        public ObservableCollection<string?> OnLoadedRequirementTitles
        {
            get { return _onLoadedRequirementTitles; }
            set
            {
                if (_onLoadedRequirementTitles != value)
                {
                    _onLoadedRequirementTitles = value;
                    OnPropertyChanged(nameof(OnLoadedRequirementTitles));
                }
            }
        }

        public ObservableCollection<string> EvaluationScoreDetails
        {
            get { return _evaluationScoreDetails; }
            set
            {
                if (_evaluationScoreDetails != value)
                {
                    _evaluationScoreDetails = value;
                    OnPropertyChanged(nameof(EvaluationScoreDetails));
                }
            }
        }

        public ObservableCollection<string> LLMList
        {
            get { return _llmList; }
        }

        public string SelectedLLM
        {
            get { return _selectedLLM; }
            set
            {
                if (_selectedLLM != value)
                {
                    _selectedLLM = value;
                    OnPropertyChanged(nameof(SelectedLLM));
                }
            }
        }

        public ObservableCollection<SpecFlowFeatureFileModel> SpecFlowFeatureFiles
        {
            get { return _specFlowFeatureFiles; }
            set
            {
                if (_specFlowFeatureFiles != value)
                {
                    _specFlowFeatureFiles = value;
                    OnPropertyChanged(nameof(SpecFlowFeatureFiles));
                }
            }
        }

        public SpecFlowFeatureFileModel SelectedSpecFlowFeatureFile
        {
            get { return _selectedSpecFlowFeatureFile; }
            set
            {
                if (_selectedSpecFlowFeatureFile != value)
                {
                    _selectedSpecFlowFeatureFile = value;
                    DisplayScoreDetails();
                    OnPropertyChanged(nameof(SelectedSpecFlowFeatureFile));
                }
            }
        }

        public ScenarioModel SelectedScenario
        {
            get { return _selectedScenario; }
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
                    DisplayScoreDetails();
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

        public void OnTreeViewItemSelected(object selectedItem)
        {
            if (selectedItem is ReqIFValueAndPath reqIFValueAndPath)
            {
                ChosenFile = reqIFValueAndPath.Path!;
            }
        }

        public void OnListBoxSelectedSpecFlowFeatureFileChanged(object selectedSpecFlowFeatureFile)
        {
            if (selectedSpecFlowFeatureFile is SpecFlowFeatureFileModel specFlowFeatureFile)
            {
                SelectedSpecFlowFeatureFile = specFlowFeatureFile;
            }
        }

        private void UploadReqIF()
        {
            StatusMessages.Add("Uploading ReqIF...");
            _ = GetReqIFFileFromFolder();
        }

        private string GetReqIFFileFromFolder()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "ReqIF (*.reqif)|*.reqif|Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            bool? response = openFileDialog.ShowDialog();

            if (response != true)
                return string.Empty;

            string filepath = openFileDialog.FileName;
            ChosenFile = filepath;

            StatusMessages.Add("ReqIF uploaded.");

            return filepath;
        }

        private async void GenerateAndEvaluateSpecFlowFeatureFile()
        {
            SelectedSpecFlowFeatureFile = new();
            SpecFlowFeatureFiles.Clear();

            StatusMessages.Add("Generating SpecFlow feature file...");

            if (string.IsNullOrEmpty(ChosenFile))
            {
                StatusMessages.Add("No file chosen.");
                return;
            }

            string requirements = GetFileContent();

            SetLLM();

            try
            {
                await GenerateSpecFlowFeatureFile(requirements);
                SelectedSpecFlowFeatureFile = SpecFlowFeatureFiles.FirstOrDefault() ?? new();
                SelectedScenario = SelectedSpecFlowFeatureFile.Scenarios.FirstOrDefault() ?? new();
                await EvaluateSpecFlowFeatureFile(requirements);
            }
            catch { return; }
        }

        public void SetLLM()
        {
            switch (SelectedLLM)
            {
                case GPT_4o.ModelName:
                    _superTestController.SelectedLLM = _gpt_4o;
                    break;
                case Claude_3_5_Sonnet.ModelName:
                    _superTestController.SelectedLLM = _claude_3_5_Sonnet;
                    break;
                case Gemini_1_5.ModelName:
                    _superTestController.SelectedLLM = _gemini_1_5;
                    break;
            }
        }

        private async Task GenerateSpecFlowFeatureFile(string requirements)
        {
            try
            {
                var featureFileResponse = await Retry.DoAsync(() => _superTestController.GenerateSpecFlowFeatureFileAsync(requirements), TimeSpan.FromSeconds(1));
                
                for (int i = 0; i < featureFileResponse.FeatureFiles.Count; i++)
                {
                    var featureFileModel = featureFileResponse.FeatureFiles.ElementAt(i);
                    var gherkinDocument = featureFileResponse.GherkinDocuments[i];

                    if (gherkinDocument == null)
                    {
                        StatusMessages.Add($"Gherkin document is missing at {featureFileModel.Key}.");
                        continue;
                    }

                    var featureFile = GetSpecFlowFeatureFileModel.ConvertSpecFlowFeatureFileResponse(featureFileModel, gherkinDocument);
                    SpecFlowFeatureFiles.Add(featureFile);
                }

                if (!SpecFlowFeatureFiles.Any())
                {
                    StatusMessages.Add("Feature file is empty. Failed to generate feature file.");
                }
                StatusMessages.Add("Finished generating!");
            }
            catch (Exception ex)
            {
                StatusMessages.Add($"Exception: {ex.Message} while generating SpecFlow feature file.");
            }
        }

        private async Task EvaluateSpecFlowFeatureFile(string requirements)
        {
            if (!SpecFlowFeatureFiles.Any())
            {
                StatusMessages.Add("No feature file to evaluate.");
                return;
            }

            foreach (var featureFile in SpecFlowFeatureFiles)
            {
                // Evaluate feature file
                StatusMessages.Add($"Evaluating {featureFile.FeatureFileName} feature file using GPT-4o...");
                await EvaluateFeatureFileScoreAsync(_gpt_4o, featureFile, requirements);

                StatusMessages.Add($"Evaluating {featureFile.FeatureFileName} feature file using Claude 3.5 Sonnet...");
                await EvaluateFeatureFileScoreAsync(_claude_3_5_Sonnet, featureFile, requirements);

                //Evaluate scenario
                StatusMessages.Add($"Evaluating {featureFile.FeatureFileName} scenario using GPT-4o...");
                await EvaluateSpecFlowScenarioAsync(_gpt_4o, featureFile, requirements);

                StatusMessages.Add($"Evaluating {featureFile.FeatureFileName} scenario using Claude 3.5 Sonnet...");
                await EvaluateSpecFlowScenarioAsync(_claude_3_5_Sonnet, featureFile, requirements);
            }
            StatusMessages.Add("Finished evaluating!");
        }

        private string GetFileContent()
        {
            try
            {
                if (File.Exists(ChosenFile))
                {
                    return File.ReadAllText(ChosenFile);
                }
                else
                {
                    StatusMessages.Add("File does not exist.");
                }
            }
            catch (IOException ex)
            {
                StatusMessages.Add($"IOException: {ex.Message} while reading {ChosenFile}");
            }
            catch (UnauthorizedAccessException ex)
            {
                StatusMessages.Add($"UnauthorizedAccessException: {ex.Message} while reading {ChosenFile}");
            }
            catch (Exception ex)
            {
                StatusMessages.Add($"Exception: {ex.Message} while processing {ChosenFile}");
            }

            return string.Empty;
        }

        private async Task EvaluateFeatureFileScoreAsync(ILargeLanguageModel largeLanguageModel, SpecFlowFeatureFileModel featureFile, string requirements)
        {
            try
            {
                _superTestController.SelectedLLM = largeLanguageModel;
                var evaluationResponse = await Retry.DoAsync(() => _superTestController.EvaluateSpecFlowFeatureFileAsync(requirements, featureFile.FeatureFileContent), TimeSpan.FromSeconds(1));
                AssignSpecFlowFeatureFileEvaluation.Assign(largeLanguageModel, featureFile, evaluationResponse);
            }
            catch (Exception ex)
            {
                StatusMessages.Add($"Exception: {ex.Message} while evaluating {featureFile.FeatureFileName} using {largeLanguageModel.Id}");
            }
        }

        private async Task EvaluateSpecFlowScenarioAsync(ILargeLanguageModel largeLanguageModel, SpecFlowFeatureFileModel featureFile, string requirements)
        {
            try
            {
                _superTestController.SelectedLLM = largeLanguageModel;
                var evaluationResponse = await Retry.DoAsync(() => _superTestController.EvaluateSpecFlowScenarioAsync(requirements, featureFile.FeatureFileContent), TimeSpan.FromSeconds(1));

                foreach (var scenario in evaluationResponse.ScenarioEvaluations)
                {
                    var scenarioModel = featureFile.Scenarios.First(s => s.Name == scenario.ScenarioName);
                    AssignScenarioEvaluation.Assign(largeLanguageModel, scenarioModel, scenario);
                }
            }
            catch (Exception ex)
            {
                StatusMessages.Add($"Exception: {ex.Message} while evaluating {featureFile.FeatureFileName} using {largeLanguageModel.Id}");
            }
        }

        private void DisplayFeatureFileScore()
        {
            _isDisplayingFeatureFileScore = true;
            DisplayScoreDetails();
        }

        private void DisplayScenarioScore()
        {
            _isDisplayingFeatureFileScore = false;
            DisplayScoreDetails();
        }

        private void DisplayScoreDetails()
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
            var folderDialog = new OpenFolderDialog
            { 
                DefaultDirectory = SavePath
            };

            if (folderDialog.ShowDialog() == true)
            {
                SavePath = folderDialog.FolderName;
            }
        }

        private void SaveFeatureFiles()
        {
            foreach (var featureFile in SpecFlowFeatureFiles)
            {
                string savePath = $"{SavePath}/{featureFile.FeatureFileName}";
                string featureFileContent = GetReviewedFeatureFile.GetAcceptedScenarios(featureFile);

                File.WriteAllText(savePath, featureFileContent);
                StatusMessages.Add($"Feature file saved to \"{savePath}\".");
            }
        }

        private void SwitchFeatureFileView()
        {
            IsFeatureFileContentVisible = !IsFeatureFileContentVisible;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
