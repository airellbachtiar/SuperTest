using Microsoft.Extensions.DependencyInjection;
using SuperTestWPF.Models;
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

        private string _savePath;
        private string _settingStatus = string.Empty;

        public ICommand SelectSaveRequirementFilesLocationCommand { get; }
        public ICommand ApplyNewRequirementFilesLocationCommand { get; }

        public string SavePath
        {
            get => _savePath;
            set => SetProperty(ref _savePath, value);
        }

        public string SettingStatus
        {
            get => _settingStatus;
            set => SetProperty(ref _settingStatus, value);
        }

        public SettingsWindowViewModel(IServiceProvider serviceProvider)
        {
            _getReqIfService = serviceProvider.GetRequiredService<IGetReqIfService>();
            _fileService = serviceProvider.GetRequiredService<IFileService>();

            SelectSaveRequirementFilesLocationCommand = new RelayCommand(SelectSaveRequirementFilesLocation);
            ApplyNewRequirementFilesLocationCommand = new RelayCommand(ApplyNewRequirementFilesLocation);

            _savePath = _getReqIfService.RequirementsStorageLocation;
        }

        private void SelectSaveRequirementFilesLocation()
        {
            SettingStatus = string.Empty;
            SavePath = _fileService.SelectFolderLocation(SavePath);
        }

        private void ApplyNewRequirementFilesLocation()
        {
            _getReqIfService.RequirementsStorageLocation = SavePath;
            SettingStatus = "Settings applied";
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