using CanvasElements;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Generated
{
    public partial class PandIView : UserControl, IDisposable
    {
        public double LoopTime
        {
            get => (double)GetValue(LoopTimeProperty);
            set => SetValue(LoopTimeProperty, value);
        }
        public static readonly DependencyProperty LoopTimeProperty = DependencyProperty.Register("LoopTime", typeof(double), typeof(PandIView));

        public double SimTime
        {
            get => (double)GetValue(SimTimeProperty);
            set => SetValue(SimTimeProperty, value);
        }
        public static readonly DependencyProperty SimTimeProperty = DependencyProperty.Register("SimTime", typeof(double), typeof(PandIView));

        public event EventHandler SelectedChanged;
        private DispatcherTimer _timer;
        private bool disposedValue;

        public PandIView(PandISimulator sim)
        {
            InitializeComponent();
            PreviewMouseLeftButtonDown += PandIView_PreviewMouseLeftButtonDown;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500),
            };
            _timer.Tick += (s, e) => Update();
            IsVisibleChanged += (s, e) => _timer.IsEnabled = IsVisible;
            theCanvas.DataContext = sim;
        }

        public UserControl View => this;

        private void PandIView_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (var c in theCanvas.Children)
            {
                if (!((UIElement) c).IsMouseOver || SelectedChanged == null)
                    continue;
                try
                {
                    SelectedChanged(c, new EventArgs());
                }
                catch
                {
                    // ignored
                }
            }
        }

        public void Update()
        {
            foreach (var child in theCanvas.Children)
            {
                if (child is IAssociationView view)
                    view.Update();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _timer = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
