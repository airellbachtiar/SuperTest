using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Microsoft.Win32;
using ReqIFSharp;

namespace SuperTestGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReqIF reqIF = new ReqIF();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UploadReqIFButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ReqIF (*.reqif)|*.reqif|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            bool? response = openFileDialog.ShowDialog();

            if (response != true) return;

            string filepath = openFileDialog.FileName;
            labelReqIFFile.Content = filepath;

            ReqIFDeserializer deserializer = new ReqIFDeserializer();
            reqIF = deserializer.Deserialize(filepath).First();

            XmlDocument doc = new XmlDocument();

            foreach (SpecObject specObject in reqIF.CoreContent.SpecObjects)
            {
                string listString = String.Join(", ", specObject.Values.Select(val => val.ObjectValue.ToString().RemoveXhtmlTags()));

                requirementsTreeViewer.Items.Add(listString);
            }
        }
    }
}