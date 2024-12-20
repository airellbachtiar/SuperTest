using SuperTestWPF.Models;
using SuperTestWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SuperTestWPF.Views
{
    /// <summary>
    /// Interaction logic for FeatureFileGeneratorView.xaml
    /// </summary>
    public partial class FeatureFileGeneratorView : UserControl
    {
        public FeatureFileGeneratorView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedItem = e.NewValue;

            if (DataContext is FeatureFileGeneratorViewModel viewModel && selectedItem != null)
            {
                viewModel.OnTreeViewItemSelected(selectedItem);
            }
        }

        private void TextBlock_SelectedScenarioChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.DataContext is ScenarioModel selectedScenario)
            {
                if (DataContext is FeatureFileGeneratorViewModel viewModel)
                {
                    viewModel.SelectedScenario = selectedScenario;
                }
            }
        }
    }
}
