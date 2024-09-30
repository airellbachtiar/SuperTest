using System.Windows;
using Microsoft.Win32;
using ReqIFSharp;
using SuperTestWPF.Helper;
using SuperTestLibrary;
using SuperTestLibrary.Storages;
using Microsoft.Extensions.Configuration;

namespace SuperTestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReqIF reqIF = new ReqIF(); 

        private string reqIFFileContent = string.Empty;

        private List<ReqIF> AllReqIFs = new List<ReqIF>();

        private List<string> AllReqIFFilesContent = new List<string>();

        private readonly ReqIFDeserializer _deserializer = new ReqIFDeserializer();

        private ISuperTestController superTestController;

        public MainWindow(ISuperTestController superTestController)
        {
            InitializeComponent();
            this.superTestController = superTestController;
            InitializeReqIFs();
        }

        private async void InitializeReqIFs()
        {
            var AllReqIFFiles = await superTestController.GetAllReqIFFilesAsync();

            foreach (var reqIFFile in AllReqIFFiles)
            {
                AllReqIFs.Add(_deserializer.Deserialize(reqIFFile).First());
                AllReqIFFilesContent.Add(System.IO.File.ReadAllText(reqIFFile));
            }

            foreach (var reqIF in AllReqIFs)
            {
                requirementsTreeViewer.Items.Add(reqIF.TheHeader.Title);
            }
        }

        private void UploadReqIFButton_Click(object sender, RoutedEventArgs e)
        {
            requirementsTreeViewer.Items.Clear();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ReqIF (*.reqif)|*.reqif|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            bool? response = openFileDialog.ShowDialog();

            if (response != true)
                return;

            string filepath = openFileDialog.FileName;
            labelReqIFFile.Content = openFileDialog;

            reqIF = _deserializer.Deserialize(filepath).First();
            reqIFFileContent = System.IO.File.ReadAllText(filepath);

            foreach (SpecObject specObject in reqIF.CoreContent.SpecObjects)
            {
                string listString = String.Join(", ", specObject.Values.Select(val => val.ObjectValue.ToString().RemoveXhtmlTags()));

                requirementsTreeViewer.Items.Add(listString);
            }
        }

        private void GenerateFeatureFileButton_Click(object sender, RoutedEventArgs e)
        {
            //TO DO: Generate feature file
        }
    }
}