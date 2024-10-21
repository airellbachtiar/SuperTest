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
        private string _featureFileSummary = string.Empty;
        private readonly ISuperTestController _superTestController;
        private readonly ObservableCollection<string> _llmList = new([GPT_4o.ModelName, Claude_3_5_Sonnet.ModelName, Gemini_1_5.ModelName]);
        private const int maxRetryCount = 3;
        private int retryCount = 0;

        // LLM
        private readonly GPT_4o _gpt_4o = new();
        private readonly Claude_3_5_Sonnet _claude_3_5_Sonnet = new();
        private readonly Gemini_1_5 _gemini_1_5 = new();

        //Generator
        private readonly SpecFlowFeatureFileGenerator _specFlowFeatureFileGenerator = new();
        private ObservableCollection<string?> _onLoadedRequirementTitles = [];
        private ObservableCollection<string?> _featureFileScoreDetails = [];

        public MainWindowViewModel(ISuperTestController superTestController)
        {
            UploadReqIFCommand = new RelayCommand(UploadReqIF);
            GenerateSpecFlowFeatureFileCommand = new RelayCommand(GenerateSpecFlowFeatureFile);
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

        public string FeatureFileSummary
        {
            get { return _featureFileSummary; }
            set
            {
                if (_featureFileSummary != value)
                {
                    _featureFileSummary = value;
                    OnPropertyChanged(nameof(FeatureFileSummary));
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

        public ObservableCollection<string?> FeatureFileScoreDetail
        {
            get { return _featureFileScoreDetails; }
            set
            {
                if (_featureFileScoreDetails != value)
                {
                    _featureFileScoreDetails = value;
                    OnPropertyChanged(nameof(FeatureFileScoreDetail));
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
            FeatureFileScoreDetail.Clear();
            FeatureFileSummary = string.Empty;
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

        public async Task GenerateSpecFlowFeatureFileCheck(string requirements)
        {
            try
            {
                var featureFileResponse = await _superTestController.GenerateSpecFlowFeatureFileAsync(requirements);

                // TODO: Support multiple output
                string? featureFile = featureFileResponse.FeatureFiles.Values.FirstOrDefault();

                if (string.IsNullOrEmpty(featureFile))
                {
                    StatusMessage = "Failed to generate SpecFlow feature file.";
                    return;
                }

                GeneratedSpecFlowFeatureFile = featureFile;
                GeneratedSpecFlowFeatureFile = featureFile;

                StatusMessage = "Evaluating SpecFlow feature file using GPT-4o...";
                await EvaluateFeatureFileScore(_gpt_4o, requirements, featureFile);
                StatusMessage = "Evaluating SpecFlow feature file using Claude 3.5 Sonnet...";
                await EvaluateFeatureFileScore(_claude_3_5_Sonnet, requirements, featureFile);

                StatusMessage = "Finish.";
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
                    StatusMessage = $"Error: Failed to generate SpecFlow feature file after 3 tries. {e.Message}";
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

        private async Task EvaluateFeatureFileScore(ILargeLanguageModel largeLanguageModel, string requirements, string featureFile)
        {
            _superTestController.SelectedLLM = largeLanguageModel;
            _superTestController.SelectedGenerator = new EvaluateSpecFlowFeatureFileGenerator(requirements);
            var evaluationResponse = await _superTestController.EvaluateSpecFlowFeatureFileAsync(featureFile);

            int totalScore = evaluationResponse.Score.TotalScore;
            int maximumScore = evaluationResponse.Score.MaximumScore;

            FeatureFileScoreDetail.Add($"{largeLanguageModel.Id} Evaluation:");
            FeatureFileScoreDetail.Add($"Readability = {evaluationResponse.Readability}/5 ");
            FeatureFileScoreDetail.Add($"Consistency = {evaluationResponse.Consistency}/5 ");
            FeatureFileScoreDetail.Add($"Focus = {evaluationResponse.Focus}/5 ");
            FeatureFileScoreDetail.Add($"Structure = {evaluationResponse.Structure}/5 ");
            FeatureFileScoreDetail.Add($"Maintainability = {evaluationResponse.Maintainability}/5 ");
            FeatureFileScoreDetail.Add($"Coverage = {evaluationResponse.Coverage}/5 ");
            FeatureFileScoreDetail.Add($"Total Score = {totalScore}/{maximumScore} ");
            FeatureFileScoreDetail.Add($"Feature file score ({largeLanguageModel.Id}): {(Convert.ToDouble(totalScore) / Convert.ToDouble(maximumScore)) * 100}% good");
            FeatureFileScoreDetail.Add(string.Empty);

            FeatureFileSummary += $"Evaluation from {largeLanguageModel.Id}:\n{evaluationResponse.Summary}\n\n";
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
