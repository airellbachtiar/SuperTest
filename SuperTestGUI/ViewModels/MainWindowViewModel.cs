using Microsoft.Win32;
using SuperTestLibrary;
using SuperTestLibrary.LLMs;
using SuperTestWPF.Helper;
using SuperTestWPF.Models;
using SuperTestWPF.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SuperTestWPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _statusMessage = string.Empty;
        private string _chosenFile = string.Empty;
        private string _selectedLLM = GPT_4o.ModelName;
        private readonly ISuperTestController _superTestController;
        private readonly ObservableCollection<string> _llmList = new([GPT_4o.ModelName, Claude_3_5_Sonnet.ModelName, Gemini_1_5.ModelName]);
        private ObservableCollection<string?> _onLoadedRequirementTitles = [];

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

        public MainWindowViewModel(ISuperTestController superTestController)
        {
            UploadReqIFCommand = new RelayCommand(UploadReqIF);
            GenerateAndEvaluateSpecFlowFeatureFileCommand = new RelayCommand(GenerateAndEvaluateSpecFlowFeatureFile);
            DisplayFeatureFileScoreCommand = new RelayCommand(DisplayFeatureFileScore);
            DisplayScenarioScoreCommand = new RelayCommand(DisplayScenarioScore);
            CopyFeatureFileCommand = new RelayCommand(CopyFeatureFile);
            SaveFeatureFileCommand = new RelayCommand(SaveFeatureFile);

            this._superTestController = superTestController;
            _ = InitializeReqIFs();
        }

        private async Task InitializeReqIFs()
        {
            var AllReqIfFiles = await _superTestController.GetAllReqIFFilesAsync();

            OnLoadedRequirementTitles = new ObservableCollection<string?>(AllReqIfFiles);
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
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

        public ICommand UploadReqIFCommand { get; }
        public ICommand GenerateAndEvaluateSpecFlowFeatureFileCommand { get; }
        public ICommand DisplayFeatureFileScoreCommand { get; }
        public ICommand DisplayScenarioScoreCommand { get; }
        public ICommand CopyFeatureFileCommand { get; }
        public ICommand SaveFeatureFileCommand { get; }

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
            StatusMessage = "Uploading ReqIF...";
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

            StatusMessage = "ReqIF uploaded.";

            return filepath;
        }

        private async void GenerateAndEvaluateSpecFlowFeatureFile()
        {
            SelectedSpecFlowFeatureFile = new();
            SpecFlowFeatureFiles.Clear();

            StatusMessage = "Generating SpecFlow feature file...";

            if (string.IsNullOrEmpty(ChosenFile))
            {
                StatusMessage = "No file chosen.";
                return;
            }

            string requirements = GetFileContent();

            SetLLM();

            try
            {
                await GenerateSpecFlowFeatureFile(requirements);
                SelectedSpecFlowFeatureFile = SpecFlowFeatureFiles.FirstOrDefault() ?? new();
                await EvaluateSpecFlowFeatureFile(requirements);
            }
            catch
            {
                StatusMessage = "Failed to generate and evaluate SpecFlow feature file.";
            }
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
                foreach (var featureFileModel in featureFileResponse.FeatureFiles)
                {
                    SpecFlowFeatureFiles.Add(new SpecFlowFeatureFileModel(featureFileModel.Key, featureFileModel.Value));
                }

                if (!SpecFlowFeatureFiles.Any())
                {
                    StatusMessage = "Feature file is empty. Failed to generate feature file.";
                }
                StatusMessage = "Finished generating!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Exception: {ex.Message} while generating SpecFlow feature file.";
            }
        }

        private async Task EvaluateSpecFlowFeatureFile(string requirements)
        {
            foreach (var featureFile in SpecFlowFeatureFiles)
            {
                // Evaluate feature file
                StatusMessage = $"Evaluating {featureFile.FeatureFileName} feature file using GPT-4o...";
                await EvaluateFeatureFileScoreAsync(_gpt_4o, featureFile, requirements);

                StatusMessage = $"Evaluating {featureFile.FeatureFileName} feature file using Claude 3.5 Sonnet...";
                await EvaluateFeatureFileScoreAsync(_claude_3_5_Sonnet, featureFile, requirements);

                //Evaluate scenario
                StatusMessage = $"Evaluating {featureFile.FeatureFileName} scenario using GPT-4o...";
                await EvaluateSpecFlowScenarioAsync(_gpt_4o, featureFile, requirements);

                StatusMessage = $"Evaluating {featureFile.FeatureFileName} scenario using Claude 3.5 Sonnet...";
                await EvaluateSpecFlowScenarioAsync(_claude_3_5_Sonnet, featureFile, requirements);
            }
            StatusMessage = "Finished evaluating!";
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
                    StatusMessage = "File does not exist.";
                }
            }
            catch (IOException ex)
            {
                StatusMessage = $"IOException: {ex.Message} while reading {ChosenFile}";
            }
            catch (UnauthorizedAccessException ex)
            {
                StatusMessage = $"UnauthorizedAccessException: {ex.Message} while reading {ChosenFile}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Exception: {ex.Message} while processing {ChosenFile}";
            }

            return string.Empty;
        }

        private async Task EvaluateFeatureFileScoreAsync(ILargeLanguageModel largeLanguageModel, SpecFlowFeatureFileModel featureFile, string requirements)
        {
            try
            {
                _superTestController.SelectedLLM = largeLanguageModel;
                var evaluationResponse = await Retry.DoAsync(() => _superTestController.EvaluateSpecFlowFeatureFileAsync(requirements, featureFile.FeatureFileContent), TimeSpan.FromSeconds(1));

                var score = evaluationResponse.Score;

                featureFile.FeatureFileEvaluationScoreDetails.Add("=========================================================================");
                featureFile.FeatureFileEvaluationScoreDetails.Add($"{largeLanguageModel.Id} Evaluation:");
                featureFile.FeatureFileEvaluationScoreDetails.Add($"Readability = {evaluationResponse.Readability}/5 ");
                featureFile.FeatureFileEvaluationScoreDetails.Add($"Consistency = {evaluationResponse.Consistency}/5 ");
                featureFile.FeatureFileEvaluationScoreDetails.Add($"Focus = {evaluationResponse.Focus}/5 ");
                featureFile.FeatureFileEvaluationScoreDetails.Add($"Structure = {evaluationResponse.Structure}/5 ");
                featureFile.FeatureFileEvaluationScoreDetails.Add($"Maintainability = {evaluationResponse.Maintainability}/5 ");
                featureFile.FeatureFileEvaluationScoreDetails.Add($"Coverage = {evaluationResponse.Coverage}/5 ");
                featureFile.FeatureFileEvaluationScoreDetails.Add(string.Empty);
                featureFile.FeatureFileEvaluationScoreDetails.Add($"Total Score = {score.TotalScore}/{score.MaximumScore} ");
                featureFile.FeatureFileEvaluationScoreDetails.Add($"Feature file score ({largeLanguageModel.Id}): {score.Percentage}% good");
                featureFile.FeatureFileEvaluationScoreDetails.Add("=========================================================================");

                featureFile.FeatureFileEvaluationSummary += "=========================================================================\n";
                featureFile.FeatureFileEvaluationSummary += $"Evaluation from {largeLanguageModel.Id}:\n{evaluationResponse.Summary}\n";
                featureFile.FeatureFileEvaluationSummary += "=========================================================================\n";
            
            }
            catch (Exception ex)
            {
                StatusMessage = $"Exception: {ex.Message} while evaluating {featureFile.FeatureFileName} using {largeLanguageModel.Id}";
            }
        }

        private async Task EvaluateSpecFlowScenarioAsync(ILargeLanguageModel largeLanguageModel, SpecFlowFeatureFileModel featureFile, string requirements)
        {
            try
            {
                _superTestController.SelectedLLM = largeLanguageModel;
                var evaluationResponse = await Retry.DoAsync(() => _superTestController.EvaluateSpecFlowScenarioAsync(requirements, featureFile.FeatureFileContent), TimeSpan.FromSeconds(1));

                featureFile.ScenarioEvaluationScoreDetails.Add("=========================================================================");

                featureFile.ScenarioEvaluationScoreDetails.Add($"{largeLanguageModel.Id} Evaluation:");

            foreach (var scenario in evaluationResponse.ScenarioEvaluations)
            {
                var score = scenario.Score;

                    featureFile.ScenarioEvaluationScoreDetails.Add("--------------------------------------------------------------------------");
                    featureFile.ScenarioEvaluationScoreDetails.Add($"Scenario: {scenario.ScenarioName}");
                    featureFile.ScenarioEvaluationScoreDetails.Add("Clarity and Readability");
                    featureFile.ScenarioEvaluationScoreDetails.Add($"\tHuman Friendly Language = {scenario.ClarityAndReadability.HumanFriendlyLanguage}/5 ");
                    featureFile.ScenarioEvaluationScoreDetails.Add($"\tConcise and Relevant Scenarios = {scenario.ClarityAndReadability.ConciseAndRelevantScenarios}/5 ");
                    featureFile.ScenarioEvaluationScoreDetails.Add($"\tLogical Flow = {scenario.ClarityAndReadability.LogicalFlow}/5 ");

                    featureFile.ScenarioEvaluationScoreDetails.Add("Structure and Focus");
                    featureFile.ScenarioEvaluationScoreDetails.Add($"\tFocused Scenario = {scenario.StructureAndFocus.FocusedScenario}/5 ");
                    featureFile.ScenarioEvaluationScoreDetails.Add($"\tScenario Structure = {scenario.StructureAndFocus.ScenarioStructure}/5 ");
                    featureFile.ScenarioEvaluationScoreDetails.Add($"\tScenario Outlines = {scenario.StructureAndFocus.ScenarioOutlines}/5 ");

                    featureFile.ScenarioEvaluationScoreDetails.Add("Maintainability");
                    featureFile.ScenarioEvaluationScoreDetails.Add($"\tMinimal Coupling to Implementation = {scenario.Maintainability.MinimalCouplingToImplementation}/5 ");
                    featureFile.ScenarioEvaluationScoreDetails.Add($"\tIndependent Scenarios = {scenario.Maintainability.IndependentScenarios}/5 ");
                    featureFile.ScenarioEvaluationScoreDetails.Add($"\tTest Data Management = {scenario.Maintainability.TestDataManagement}/5 ");

                    featureFile.ScenarioEvaluationScoreDetails.Add("Traceability");
                    featureFile.ScenarioEvaluationScoreDetails.Add($"\tTraceability = {scenario.Traceability.TraceabilityToRequirements}/5 ");

                    featureFile.ScenarioEvaluationScoreDetails.Add(string.Empty);
                    featureFile.ScenarioEvaluationScoreDetails.Add($"Total Score = {score.TotalScore}/{score.MaximumScore} ");
                    featureFile.ScenarioEvaluationScoreDetails.Add($"Feature file score ({largeLanguageModel.Id}): {score.Percentage}% good");
                    featureFile.ScenarioEvaluationScoreDetails.Add("--------------------------------------------------------------------------"); 

                    featureFile.ScenarioEvaluationSummary += "=========================================================================\n";
                    featureFile.ScenarioEvaluationSummary += $"({largeLanguageModel.Id})Scenario: {scenario.ScenarioName}\n{scenario.Summary}\n";
                    featureFile.ScenarioEvaluationSummary += "=========================================================================\n";
                }
                featureFile.ScenarioEvaluationScoreDetails.Add("=========================================================================");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Exception: {ex.Message} while evaluating {featureFile.FeatureFileName} using {largeLanguageModel.Id}";
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
                EvaluationSummary = SelectedSpecFlowFeatureFile.ScenarioEvaluationSummary ?? string.Empty;
                EvaluationScoreDetails = SelectedSpecFlowFeatureFile.ScenarioEvaluationScoreDetails ?? [];
            }
        }

        private void CopyFeatureFile()
        {
            if (string.IsNullOrEmpty(SelectedSpecFlowFeatureFile.ToString())) return;
            Clipboard.SetText(_selectedSpecFlowFeatureFile.FeatureFileContent);
            StatusMessage = "Feature file copied to clipboard.";
        }

        private void SaveFeatureFile()
        {
            if (string.IsNullOrEmpty(SelectedSpecFlowFeatureFile.ToString())) return;
            string downloadPath = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
            File.WriteAllText($"{downloadPath}/{SelectedSpecFlowFeatureFile.FeatureFileName}", SelectedSpecFlowFeatureFile.FeatureFileContent);
            StatusMessage = $"Feature file saved to \"{downloadPath}/{SelectedSpecFlowFeatureFile.FeatureFileName}\".";
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
