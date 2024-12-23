using Generated;
using System.Windows;
using Tools.Attributes;
using TrafficSim.Generated;
using TrafficSim.ViewModel;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace TrafficSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly PandIView _fluidSimulatorView;
        private readonly MotionSimulatorUI _motionSimulatorView;

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            _fluidSimulatorView = new PandIView(viewModel.FluidSimulator);
            FluidSystemViewContainer.Content = _fluidSimulatorView;
            _fluidSimulatorView.SelectedChanged += MainWindow_SelectedChanged;
            _motionSimulatorView = new MotionSimulatorUI { DataContext = viewModel.MotionSimulator };
            MotionSystemViewContainer.Content = _motionSimulatorView;
            _motionSimulatorView.SelectedChanged += MainWindow_SelectedChanged;

            StartUpdates();
        }

        private void MainWindow_SelectedChanged(object selectedObject, EventArgs e)
        {
            if (selectedObject is FrameworkElement fe)
                UpdatePropertyGrid(fe.DataContext);
        }

        private void UpdatePropertyGrid(object mel)
        {
            if (mel == null)
                return;

            PropertiesViewer.PropertyDefinitions.Clear();
            var pd = new PropertyDefinition { TargetProperties = new List<string> { "ElementName" } };
            foreach (var p in mel.GetType().GetProperties())
            {
                var attrs = p.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    if (attr is SimulatedValueVisibleAttribute)
                    {
                        pd.TargetProperties.Add(p.Name);
                    }
                }
            }
            PropertiesViewer.PropertyDefinitions.Add(pd);
            PropertiesViewer.SelectedObject = mel;
        }

        protected override void OnClosed(EventArgs e)
        {
            StopUpdates();
            _fluidSimulatorView.Dispose();
            base.OnClosed(e);
        }

        private void StartUpdates()
        {
            Task.Run(Update);
        }
        private async Task Update()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _fluidSimulatorView.Update();
                    _motionSimulatorView.Update();
                });
                await Task.Delay(TimeSpan.FromMilliseconds(150));
            }
        }

        private void StopUpdates()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}