using Microsoft.Win32;
using SuperTestLibrary;
using SuperTestWPF.Converters;
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
        private readonly ISuperTestController _superTestController;

        private ObservableCollection<string?> _requirementSpecifications = new ObservableCollection<string?> { };
        private ObservableCollection<string?> _onLoadedRequirementTitles = new ObservableCollection<string?> { };

        public MainWindowViewModel(ISuperTestController superTestController)
        {
            UploadReqIFCommand = new RelayCommand(UploadReqIF);
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

        public ObservableCollection<string?> RequirementSpecifications
        {
            get { return _requirementSpecifications; }
            set {
                if (_requirementSpecifications != value)
                {
                    _requirementSpecifications = value;
                    OnPropertyChanged(nameof(RequirementSpecifications));
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

        public ICommand UploadReqIFCommand { get; }

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

            if (string.IsNullOrEmpty(ChosenFile))
            {
                StatusMessage = "No file chosen.";
            }

            return filepath;
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
