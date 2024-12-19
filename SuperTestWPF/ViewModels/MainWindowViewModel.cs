using System.Collections.ObjectModel;

namespace SuperTestWPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<object> Tabs { get; set; }

        private object _selectedTab;
        public object SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            Tabs =
            [
                new FeatureFileGeneratorViewModel(serviceProvider),
                new BindingFileGeneratorViewModel(serviceProvider),
                new RequirementGeneratorViewModel(serviceProvider),
                new PromptVerboseViewModel(serviceProvider)
            ];
            _selectedTab = Tabs.First();
        }
    }
}
