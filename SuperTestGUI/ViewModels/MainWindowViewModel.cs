using Microsoft.Win32;
using ReqIFSharp;
using SuperTestLibrary;
using SuperTestWPF.Helper;
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
        private readonly ReqIFDeserializer _deserializer;
        private ISuperTestController superTestController;
        public ObservableCollection<string> Requirements { get; }
        public ObservableCollection<ReqIF> ReqIFFiles { get; }
        public ObservableCollection<string> ReqIFFileTitles { get; }

        public MainWindowViewModel(ISuperTestController superTestController)
        {
            UploadReqIFCommand = new RelayCommand(UploadReqIF);
            _deserializer = new ReqIFDeserializer();
            Requirements = new ObservableCollection<string>();
            ReqIFFiles = new ObservableCollection<ReqIF>();
            ReqIFFileTitles = new ObservableCollection<string>();
            this.superTestController = superTestController;
            InitializeReqIFs();
        }

        private async void InitializeReqIFs()
        {
            var AllReqIFFiles = await superTestController.GetAllReqIFFilesAsync();

            foreach (var reqIFFile in AllReqIFFiles)
            {
                ReqIFFiles.Add(_deserializer.Deserialize(reqIFFile).First());
            }

            foreach (var reqIF in ReqIFFiles)
            {
                ReqIFFileTitles.Add(reqIF.TheHeader.Title);
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

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "ReqIF (*.reqif)|*.reqif|Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            bool? response = openFileDialog.ShowDialog();

            if (response != true)
                return;

            string filepath = openFileDialog.FileName;
            ChosenFile = filepath;

            Requirements.Clear();

            var reqIF = _deserializer.Deserialize(filepath).FirstOrDefault();

            if (reqIF == null)
            {
                StatusMessage = "Failed to load ReqIF file.";
                return;
            }

            // Extract the SpecObjects and add them to the ObservableCollection
            foreach (var specObject in reqIF.CoreContent.SpecObjects)
            {
                string listString = string.Join(", ", specObject.Values.Select(val => val.ObjectValue.ToString().RemoveXhtmlTags()));
                Requirements.Add(listString);
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
