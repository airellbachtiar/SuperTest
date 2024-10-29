using System.Windows;
using SuperTestWPF.ViewModels;

namespace SuperTestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedItem = e.NewValue;
            var viewModel = DataContext as MainWindowViewModel;

            if (viewModel != null && selectedItem != null)
            {
                viewModel.OnTreeViewItemSelected(selectedItem);
            }
        }
    }
}