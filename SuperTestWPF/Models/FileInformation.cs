using System.ComponentModel;

namespace SuperTestWPF.Models
{
    public class FileInformation : INotifyPropertyChanged
    {
        private string? value;
        private string? path;

        public FileInformation(string? value, string? path)
        {
            Value = value;
            Path = path;
        }

        public string? Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public string? Path
        {
            get => path;
            set
            {
                if (path != value)
                {
                    path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        public override string ToString()
        {
            return Value ?? string.Empty;
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
