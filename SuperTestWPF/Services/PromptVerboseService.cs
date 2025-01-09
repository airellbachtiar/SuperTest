using SuperTestWPF.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SuperTestWPF.Services
{
    public class PromptVerboseService : INotifyPropertyChanged, IPromptVerboseService
    {
        private ObservableCollection<PromptHistory> _promptHistories = [];
        public ObservableCollection<PromptHistory> PromptHistories
        {
            get => _promptHistories;
            set => SetProperty(ref _promptHistories, value);
        }

        public void AddPrompt(PromptHistory prompt)
        {
            PromptHistories.Add(prompt);
        }

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
    }
}
