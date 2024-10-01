using System.ComponentModel;

namespace SuperTestWPF.Models
{
    public class RequirementSpecification : INotifyPropertyChanged
    {
        private string? _title;
        private IEnumerable<string>? _requirements;

        public RequirementSpecification(string? title, IEnumerable<string>? requirements)
        {
            Title = title;
            Requirements = requirements;
        }

        public string? Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public IEnumerable<string>? Requirements
        {
            get => _requirements;
            set
            {
                if (_requirements != value)
                {
                    _requirements = value;
                    OnPropertyChanged(nameof(Requirements));
                }
            }
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
