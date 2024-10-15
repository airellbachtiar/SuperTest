using Microsoft.Win32;
using SuperTestLibrary;
using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services;
using SuperTestLibrary.Services.Prompts.Builders;
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
        private string _selectedLLM = Claude_3_5_Sonnet.ModelName;
        private string _generatedSpecFlowFeatureFile = string.Empty;
        private readonly ISuperTestController _superTestController;
        private readonly ObservableCollection<string> _llmList = new ObservableCollection<string>([GPT_4o.ModelName, Claude_3_5_Sonnet.ModelName, Gemini_1_5.ModelName]);

        private ObservableCollection<string?> _onLoadedRequirementTitles = new ObservableCollection<string?> ();

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

            string reqIfPath = GetReqIFFileFromFolder();
        }

        private string GetReqIFFileFromFolder()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
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
            StatusMessage = "Generating SpecFlow feature file...";

            if (string.IsNullOrEmpty(ChosenFile))
            {
                StatusMessage = "No file chosen.";
                return;
            }

            string chosenFileContent = GetFileContent();

            _superTestController.SetGenerator(new SpecFlowFeatureFileGenerator());

            switch (_selectedLLM)
            {
                case GPT_4o.ModelName:
                    _superTestController.SetLLM(new GPT_4o());
                    break;
                case Claude_3_5_Sonnet.ModelName:
                    _superTestController.SetLLM(new Claude_3_5_Sonnet());
                    break;
                case Gemini_1_5.ModelName:
                    _superTestController.SetLLM(new Gemini_1_5());
                    break;
            }

            var featureFileResponse = await _superTestController.GenerateSpecFlowFeatureFileAsync(chosenFileContent);

            // TODO: Support multiple output
            string? featureFile = featureFileResponse.FeatureFiles.Values.FirstOrDefault();

            if (string.IsNullOrEmpty(featureFile))
            {
                StatusMessage = "Failed to generate SpecFlow feature file.";
                return;
            }

            GeneratedSpecFlowFeatureFile = featureFile;

            StatusMessage = "SpecFlow feature file generated.";
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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
