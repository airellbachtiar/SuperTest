using System.Windows;
using System.Windows.Controls;
using SuperTestWPF.Models;
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

            if (DataContext is MainWindowViewModel viewModel && selectedItem != null)
            {
                viewModel.OnTreeViewItemSelected(selectedItem);
            }
        }

        private void TextBlock_SelectedScenarioChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.DataContext is ScenarioModel selectedScenario)
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.SelectedScenario = selectedScenario;
                }
            }
        }
    }
}