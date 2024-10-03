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
        private string _statusMessage = "";
        private string _chosenFile = "No file chosen";
        private readonly ISuperTestController _superTestController;
        private readonly ReqIfUriToRequirementSpecificationConverter _reqIfUriToRequirementSpecificationConverter = new ReqIfUriToRequirementSpecificationConverter();

        public ObservableCollection<string?> RequirementSpecifications { get; } = new ObservableCollection<string?> { };
        public ObservableCollection<string?> OnLoadedRequirementTitles { get; } = new ObservableCollection<string?> { };

        public MainWindowViewModel(ISuperTestController superTestController)
        {
            UploadReqIFCommand = new RelayCommand(UploadReqIF);
            this._superTestController = superTestController;
            InitializeReqIFs();
        }

        private async void InitializeReqIFs()
        {
            var AllReqIfFiles = await _superTestController.GetAllReqIFFilesAsync();

            foreach (var reqIfFile in AllReqIfFiles)
            {
                RequirementSpecification? requirement = _reqIfUriToRequirementSpecificationConverter.Convert(reqIfFile);
                if (requirement != null)
                {
                    OnLoadedRequirementTitles.Add(requirement.Title);
                }
            }
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

        public ICommand UploadReqIFCommand { get; }

        private void UploadReqIF()
        {
            StatusMessage = "Uploading ReqIF...";

            string reqIfPath = GetReqIFFileFromFolder();

            if (string.IsNullOrEmpty(reqIfPath))
            {
                StatusMessage = "No file chosen.";
                return;
            }
            RequirementSpecifications.Clear();

            GetRequirementsFromReqIF(reqIfPath);
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

        private void GetRequirementsFromReqIF(string reqIfPath)
        {
            RequirementSpecification? requirement = _reqIfUriToRequirementSpecificationConverter.Convert(reqIfPath);

            if (requirement == null)
            {
                StatusMessage = "Failed to load ReqIF file.";
                return;
            }
            else if (requirement.Requirements == null)
            {
                StatusMessage = "Failed to load requirements.";
                return;
            }

            foreach (var req in requirement.Requirements)
            {
                RequirementSpecifications.Add(req);
            }

            StatusMessage = "ReqIF upload complete.";
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
