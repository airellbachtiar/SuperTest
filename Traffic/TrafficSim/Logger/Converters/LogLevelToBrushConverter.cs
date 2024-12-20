using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Logger;

namespace TrafficSim.Logger.Converters
{
    public class LogLevelToBrushConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogLevel level)
            {
                switch (level)
                {
                    case LogLevel.Error:
                        return new SolidColorBrush(Colors.Red);
                    case LogLevel.Warning:
                        return new SolidColorBrush(Colors.Orange);
                    case LogLevel.Information:
                        return new SolidColorBrush(Colors.DarkGray);
                }
            }
            throw new ArgumentException("not all enum values covered");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
