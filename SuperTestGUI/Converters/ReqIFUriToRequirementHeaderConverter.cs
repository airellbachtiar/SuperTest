using ReqIFSharp;
using SuperTestWPF.Models;
using System.Globalization;
using System.Windows.Data;

namespace SuperTestWPF.Converters
{
    public class ReqIFUriToRequirementHeaderConverter : IValueConverter
    {
        private readonly ReqIFDeserializer _reqIfDeserializer = new ReqIFDeserializer();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<ReqIFValueAndPath> output = new List<ReqIFValueAndPath>();

            // Value is a collection of strings
            foreach (var item in (value as IEnumerable<string>)!)
            {
                ReqIF? reqIf = _reqIfDeserializer.Deserialize(item).FirstOrDefault();

                if (reqIf != null)
                {
                    output.Add(new ReqIFValueAndPath(reqIf.TheHeader.Title, item));
                }
            }

            return output;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string?> paths = new List<string?>();

            // Value is a collection of ReqIFValueAndPath
            foreach (var item in (value as IEnumerable<ReqIFValueAndPath>)!)
            {
                paths.Add(item.Path);
            }

            return paths;
        }
    }
}
