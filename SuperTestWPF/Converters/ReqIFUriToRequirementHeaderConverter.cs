using ReqIFSharp;
using SuperTestWPF.Models;
using System.Globalization;
using System.Windows.Data;

namespace SuperTestWPF.Converters
{
    public class ReqIFUriToRequirementHeaderConverter : IValueConverter
    {
        private readonly ReqIFDeserializer _reqIfDeserializer = new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<FileInformation> output = [];

            // Value is a collection of strings
            foreach (var item in (value as IEnumerable<string>)!)
            {
                ReqIF? reqIf = _reqIfDeserializer.Deserialize(item).FirstOrDefault();

                if (reqIf != null)
                {
                    output.Add(new FileInformation(item, reqIf.TheHeader.Title));
                }
            }

            return output;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string?> paths = [];

            // Value is a collection of FileInformation
            foreach (var item in (value as IEnumerable<FileInformation>)!)
            {
                paths.Add(item.Path);
            }

            return paths;
        }
    }
}
