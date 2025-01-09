using SuperTestWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SuperTestWPF.Views
{
    /// <summary>
    /// Interaction logic for BindingFileGeneratorView.xaml
    /// </summary>
    public partial class BindingFileGeneratorView : UserControl
    {
        public BindingFileGeneratorView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedItem = e.NewValue;

            if (DataContext is BindingFileGeneratorViewModel viewModel && selectedItem != null)
            {
                viewModel.OnTreeViewItemSelected(selectedItem);
            }
        }
    }
}
