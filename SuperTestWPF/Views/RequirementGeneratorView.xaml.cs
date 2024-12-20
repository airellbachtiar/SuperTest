using SuperTestWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SuperTestWPF.Views
{
    /// <summary>
    /// Interaction logic for RequirementGeneratorView.xaml
    /// </summary>
    public partial class RequirementGeneratorView : UserControl
    {
        public RequirementGeneratorView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedTestFileChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedItem = e.NewValue;

            if (DataContext is RequirementGeneratorViewModel viewModel && selectedItem != null)
            {
                viewModel.OnTestFileSelected(selectedItem);
            }
        }
    }
}
