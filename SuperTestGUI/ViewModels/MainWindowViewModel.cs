using Microsoft.Win32;
using SuperTestLibrary;
using SuperTestLibrary.Services;
using SuperTestWPF.Converters;
using SuperTestWPF.Enums;
using SuperTestWPF.Models;
using SuperTestWPF.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace SuperTestWPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _statusMessage = string.Empty;
        private string _chosenFile = string.Empty;
        private string _chosenFileContent = "";
        private LLMTypes _selectedLLM = LLMTypes.GPT_4o;
        private readonly ISuperTestController _superTestController;
        private readonly ObservableCollection<LLMTypes> _llmList = new ObservableCollection<LLMTypes> { LLMTypes.GPT_4o, LLMTypes.Claude_3_5_Sonnet, LLMTypes.Gemini_1_5 };

        private ObservableCollection<string?> _requirementSpecifications = new ObservableCollection<string?> { };
        private ObservableCollection<string?> _onLoadedRequirementTitles = new ObservableCollection<string?> { };

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

        public ObservableCollection<LLMTypes> LLMList
        {
            get { return _llmList; }
        }

        public LLMTypes SelectedLLM
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

        public ICommand UploadReqIFCommand { get; }
        public ICommand GenerateSpecFlowFeatureFileCommand { get; }

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

            return filepath;
        }

        private void GenerateSpecFlowFeatureFile()
        {
            StatusMessage = "Generating SpecFlow feature file...";

            string featureFile = _superTestController.GenerateSpecFlowFeatureFile();

            if (string.IsNullOrEmpty(featureFile))
            {
                StatusMessage = "Failed to generate SpecFlow feature file.";
                return;
            }

            StatusMessage = "SpecFlow feature file generated.";
        }

        private void GenerateSpecFlowFeatureFile()
        {
            StatusMessage = "Generating SpecFlow feature file...";

            if(string.IsNullOrEmpty(_chosenFileContent))
            {
                StatusMessage = "No file chosen.";
                return;
            }

            switch (_selectedLLM)
            {
                case LLMTypes.GPT_4o:
                    //_superTestController.SetLLM(new GPT_4o());
                    throw new System.NotImplementedException();
                case LLMTypes.Claude_3_5_Sonnet:
                    //_superTestController.SetLLM(new Claude_3_5_Sonnet());
                    throw new System.NotImplementedException();
                case LLMTypes.Gemini_1_5:
                    _superTestController.SetLLM(new Gemini1_5());
                    break;
            }

            string featureFile = _superTestController.GenerateSpecFlowFeatureFileAsync(_chosenFileContent).Result;

            if (string.IsNullOrEmpty(featureFile))
            {
                StatusMessage = "Failed to generate SpecFlow feature file.";
                return;
            }

            StatusMessage = "SpecFlow feature file generated.";
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
