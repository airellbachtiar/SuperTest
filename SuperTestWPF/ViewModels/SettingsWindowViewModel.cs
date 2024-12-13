using Microsoft.Extensions.DependencyInjection;
using SuperTestWPF.Services;
using SuperTestWPF.ViewModels.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SuperTestWPF.ViewModels
{
    public class SettingsWindowViewModel : INotifyPropertyChanged
    {
        private readonly IGetReqIfService _getReqIfService;
        private readonly IFileService _fileService;

        public string SavePath { get; set; } = string.Empty;
        public ICommand SelectSaveRequirementFilesLocationCommand { get; }
        public ICommand ApplyNewRequirementFilesLocationCommand { get; }

        public SettingsWindowViewModel(IServiceProvider serviceProvider)
        {
            _getReqIfService = serviceProvider.GetRequiredService<IGetReqIfService>();
            _fileService = serviceProvider.GetRequiredService<IFileService>();

            SelectSaveRequirementFilesLocationCommand = new RelayCommand(SelectSaveRequirementFilesLocation);
            ApplyNewRequirementFilesLocationCommand = new RelayCommand(ApplyNewRequirementFilesLocation);
        }

        private void SelectSaveRequirementFilesLocation()
        {
            SavePath = _fileService.SelectFolderLocation(SavePath);
        }

        private void ApplyNewRequirementFilesLocation()
        {
            // Save the new path to the settings file
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}