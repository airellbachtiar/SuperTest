using Microsoft.Win32;
using SuperTestLibrary;
using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services;
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
        private string _statusMessage = string.Empty;
        private string _chosenFile = string.Empty;
        private string _selectedLLM = GPT_4o.ModelName;
        private string _generatedSpecFlowFeatureFile = string.Empty;
        private readonly ISuperTestController _superTestController;
        private readonly ObservableCollection<string> _llmList = new([GPT_4o.ModelName, Claude_3_5_Sonnet.ModelName, Gemini_1_5.ModelName]);
        private ObservableCollection<string?> _onLoadedRequirementTitles = [];
        private const int maxRetryCount = 3;
        private int retryCount = 0;

        // LLM
        private readonly GPT_4o _gpt_4o = new();
        private readonly Claude_3_5_Sonnet _claude_3_5_Sonnet = new();
        private readonly Gemini_1_5 _gemini_1_5 = new();

        //Generator
        private readonly SpecFlowFeatureFileGenerator _specFlowFeatureFileGenerator = new();

        //Evaluation
        private string _featureFileEvaluationSummary = string.Empty;
        private string _scenarioEvaluationSummary = string.Empty;
        private ObservableCollection<string?> _featureFileEvaluationScoreDetails = [];
        private ObservableCollection<string?> _scenarioEvaluationScoreDetails = [];
        private string _evaluationSummary = string.Empty;
        private ObservableCollection<string?> _evaluationScoreDetails = [];

        public MainWindowViewModel(ISuperTestController superTestController)
        {
            UploadReqIFCommand = new RelayCommand(UploadReqIF);
            GenerateSpecFlowFeatureFileCommand = new RelayCommand(GenerateSpecFlowFeatureFile);
            DisplayFeatureFileScoreCommand = new RelayCommand(DisplayFeatureFileScore);
            DisplayScenarioScoreCommand = new RelayCommand(DisplayScenarioScore);

            this._superTestController = superTestController;
            InitializeReqIFs();
        }

        private async void InitializeReqIFs()
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

        public ObservableCollection<string?> EvaluationScoreDetails
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

        public string GeneratedSpecFlowFeatureFile
        {
            get { return _generatedSpecFlowFeatureFile; }
            set
            {
                if (_generatedSpecFlowFeatureFile != value)
                {
                    _generatedSpecFlowFeatureFile = value;
                    OnPropertyChanged(nameof(GeneratedSpecFlowFeatureFile));
                }
            }
        }

        public ICommand UploadReqIFCommand { get; }
        public ICommand GenerateSpecFlowFeatureFileCommand { get; }
        public ICommand DisplayFeatureFileScoreCommand { get; }
        public ICommand DisplayScenarioScoreCommand { get; }

        public void OnTreeViewItemSelected(object selectedItem)
        {
            if (selectedItem is ReqIFValueAndPath reqIFValueAndPath)
            {
                ChosenFile = reqIFValueAndPath.Path!;
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

        private async void GenerateSpecFlowFeatureFile()
        {
            _featureFileEvaluationScoreDetails.Clear();
            _scenarioEvaluationScoreDetails.Clear();
            EvaluationScoreDetails.Clear();
            _featureFileEvaluationSummary = string.Empty;
            _scenarioEvaluationSummary = string.Empty;
            EvaluationSummary = string.Empty;

            GeneratedSpecFlowFeatureFile = string.Empty;

            StatusMessage = "Generating SpecFlow feature file...";

            if (string.IsNullOrEmpty(ChosenFile))
            {
                StatusMessage = "No file chosen.";
                return;
            }

            string requirements = GetFileContent();

            SetLLM();
            _superTestController.SelectedGenerator = _specFlowFeatureFileGenerator;

            await GenerateSpecFlowFeatureFileCheck(requirements);
            await EvaluateSpecFlowFeatureFile(requirements);

            StatusMessage = "Finished generating and evaluating!";

            retryCount = 0;
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

        private async Task GenerateSpecFlowFeatureFileCheck(string requirements)
        {
            try
            {
                var featureFileResponse = await _superTestController.GenerateSpecFlowFeatureFileAsync(requirements);

                // TODO: Support multiple output
                string? featureFile = featureFileResponse.FeatureFiles.Values.FirstOrDefault();

                if (string.IsNullOrEmpty(featureFile))
                {
                    StatusMessage = "Feature file is empty. Failed to generate feature file.";
                    return;
                }

                GeneratedSpecFlowFeatureFile = featureFile;
            }
            catch (Exception e)
            {
                if (retryCount < maxRetryCount)
                {
                    StatusMessage = $"Generation encounters error. ({retryCount + 1})";
                    retryCount++;
                    await GenerateSpecFlowFeatureFileCheck(requirements);
                }
                else
                {
                    retryCount = 0;
                    StatusMessage = $"Error: Failed to generate SpecFlow feature file after 3 tries. {e.Message}";
                }
            }
        }

        private async Task EvaluateSpecFlowFeatureFile(string requirements)
        {
            try
            {
                // Evaluate feature file
                _superTestController.SelectedGenerator = new EvaluateSpecFlowFeatureFileGenerator(requirements);
                StatusMessage = "Evaluating SpecFlow feature file using GPT-4o...";
                await EvaluateFeatureFileScoreAsync(_gpt_4o);
                StatusMessage = "Evaluating SpecFlow feature file using Claude 3.5 Sonnet...";
                await EvaluateFeatureFileScoreAsync(_claude_3_5_Sonnet);

                //Evaluate scenario
                _superTestController.SelectedGenerator = new EvaluateSpecFlowScenarioGenerator(requirements);
                StatusMessage = "Evaluating SpecFlow scenario using GPT-4o...";
                await EvaluateSpecFlowScenarioAsync(_gpt_4o);
                StatusMessage = "Evaluating SpecFlow scenario using Claude 3.5 Sonnet...";
                await EvaluateSpecFlowScenarioAsync(_claude_3_5_Sonnet);
            }
            catch (Exception e)
            {
                if (retryCount < maxRetryCount)
                {
                    StatusMessage = $"Evaluation encounters error. ({retryCount + 1})";
                    retryCount++;
                    await EvaluateSpecFlowFeatureFile(requirements);
                }
                else
                {
                    retryCount = 0;
                    StatusMessage = $"Error: Failed to evaluate SpecFlow feature file after 3 tries. {e.Message}";
                }
            }
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

        private async Task EvaluateFeatureFileScoreAsync(ILargeLanguageModel largeLanguageModel)
        {
            _superTestController.SelectedLLM = largeLanguageModel;
            var evaluationResponse = await _superTestController.EvaluateSpecFlowFeatureFileAsync(GeneratedSpecFlowFeatureFile);

            var score = evaluationResponse.Score;

            _featureFileEvaluationScoreDetails.Add("=========================================================================");
            _featureFileEvaluationScoreDetails.Add($"{largeLanguageModel.Id} Evaluation:");
            _featureFileEvaluationScoreDetails.Add($"Readability = {evaluationResponse.Readability}/5 ");
            _featureFileEvaluationScoreDetails.Add($"Consistency = {evaluationResponse.Consistency}/5 ");
            _featureFileEvaluationScoreDetails.Add($"Focus = {evaluationResponse.Focus}/5 ");
            _featureFileEvaluationScoreDetails.Add($"Structure = {evaluationResponse.Structure}/5 ");
            _featureFileEvaluationScoreDetails.Add($"Maintainability = {evaluationResponse.Maintainability}/5 ");
            _featureFileEvaluationScoreDetails.Add($"Coverage = {evaluationResponse.Coverage}/5 ");
            _featureFileEvaluationScoreDetails.Add(string.Empty);
            _featureFileEvaluationScoreDetails.Add($"Total Score = {score.TotalScore}/{score.MaximumScore} ");
            _featureFileEvaluationScoreDetails.Add($"Feature file score ({largeLanguageModel.Id}): {score.Percentage}% good");
            _featureFileEvaluationScoreDetails.Add("=========================================================================");

            _featureFileEvaluationSummary += "=========================================================================\n";
            _featureFileEvaluationSummary += $"Evaluation from {largeLanguageModel.Id}:\n{evaluationResponse.Summary}\n";
            _featureFileEvaluationSummary += "=========================================================================\n";
        }

        private async Task EvaluateSpecFlowScenarioAsync(ILargeLanguageModel largeLanguageModel)
        {
            _superTestController.SelectedLLM = largeLanguageModel;
            var evaluationResponse = await _superTestController.EvaluateSpecFlowScenarioAsync(GeneratedSpecFlowFeatureFile);

            _scenarioEvaluationScoreDetails.Add("=========================================================================");

            _scenarioEvaluationScoreDetails.Add($"{largeLanguageModel.Id} Evaluation:");

            foreach (var scenario in evaluationResponse.ScenarioEvaluations)
            {
                int totalScore = scenario.Score.TotalScore;
                int maximumScore = scenario.Score.MaximumScore;

                _scenarioEvaluationScoreDetails.Add("--------------------------------------------------------------------------");
                _scenarioEvaluationScoreDetails.Add($"Scenario: {scenario.ScenarioName}");
                _scenarioEvaluationScoreDetails.Add("Clarity and Readability");
                _scenarioEvaluationScoreDetails.Add($"\tHuman Friendly Language = {scenario.ClarityAndReadability.HumanFriendlyLanguage}/5 ");
                _scenarioEvaluationScoreDetails.Add($"\tConcise and Relevant Scenarios = {scenario.ClarityAndReadability.ConciseAndRelevantScenarios}/5 ");
                _scenarioEvaluationScoreDetails.Add($"\tLogical Flow = {scenario.ClarityAndReadability.LogicalFlow}/5 ");

                _scenarioEvaluationScoreDetails.Add("Structure and Focus");
                _scenarioEvaluationScoreDetails.Add($"\tFocused Scenario = {scenario.StructureAndFocus.FocusedScenario}/5 ");
                _scenarioEvaluationScoreDetails.Add($"\tScenario Structure = {scenario.StructureAndFocus.ScenarioStructure}/5 ");
                _scenarioEvaluationScoreDetails.Add($"\tScenario Outlines = {scenario.StructureAndFocus.ScenarioOutlines}/5 ");

                _scenarioEvaluationScoreDetails.Add("Maintainability");
                _scenarioEvaluationScoreDetails.Add($"\tMinimal Coupling to Implementation = {scenario.Maintainability.MinimalCouplingToImplementation}/5 ");
                _scenarioEvaluationScoreDetails.Add($"\tIndependent Scenarios = {scenario.Maintainability.IndependentScenarios}/5 ");
                _scenarioEvaluationScoreDetails.Add($"\tTest Data Management = {scenario.Maintainability.TestDataManagement}/5 ");

                _scenarioEvaluationScoreDetails.Add("Traceability");
                _scenarioEvaluationScoreDetails.Add($"\tTraceability = {scenario.Traceability.TraceabilityToRequirements}/5 ");

                _scenarioEvaluationScoreDetails.Add(string.Empty);
                _scenarioEvaluationScoreDetails.Add($"Total Score = {totalScore}/{maximumScore} ");
                _scenarioEvaluationScoreDetails.Add($"Feature file score ({largeLanguageModel.Id}): {(Convert.ToDouble(totalScore) / Convert.ToDouble(maximumScore)) * 100}% good");
                _scenarioEvaluationScoreDetails.Add("--------------------------------------------------------------------------");

                _scenarioEvaluationSummary += "=========================================================================\n";
                _scenarioEvaluationSummary += $"({largeLanguageModel.Id})Scenario: {scenario.ScenarioName}\n{scenario.Summary}\n";
                _scenarioEvaluationSummary += "=========================================================================\n";
            }
            _scenarioEvaluationScoreDetails.Add("=========================================================================");
        }

        private void DisplayFeatureFileScore()
        {
            EvaluationSummary = _featureFileEvaluationSummary;
            EvaluationScoreDetails = _featureFileEvaluationScoreDetails;
        }

        private void DisplayScenarioScore()
        {
            EvaluationSummary = _scenarioEvaluationSummary;
            EvaluationScoreDetails = _scenarioEvaluationScoreDetails;
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
