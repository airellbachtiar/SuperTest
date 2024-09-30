using System.Windows;
using ReqIFSharp;
using SuperTestLibrary;
using SuperTestWPF.ViewModels;

namespace SuperTestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow(ISuperTestController superTestController)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(superTestController);
        }
    }
}