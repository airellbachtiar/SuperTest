using ReqIFSharp;
using SuperTestWPF.Helper;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace SuperTestWPF.Converters
{
    public class ReqIfUriToRequirementsConverter : IValueConverter
    {
        private readonly ReqIFDeserializer _reqIfDeserializer = new ReqIFDeserializer();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Value is a string
            string? reqIfPath = value.ToString();

            if (string.IsNullOrEmpty(reqIfPath))
            {
                return null;
            }

            try
            {
                ReqIF? reqIf = _reqIfDeserializer.Deserialize(reqIfPath).FirstOrDefault();

                if (reqIf == null)
                {
                    return null;
                }

                string header = reqIf.TheHeader.Title;
                List<string> requirements = new List<string>();

                foreach (var specObject in reqIf.CoreContent.SpecObjects)
                {
                    string listString = string.Join(", ", specObject.Values.Select(val => val.ObjectValue.ToString().RemoveXhtmlTags()));
                    requirements.Add(listString);
                }

                ObservableCollection<string> observableRequirements = new ObservableCollection<string>(requirements);

                return observableRequirements;
            }
            catch
            {
                throw new Exception("Error while converting ReqIF to RequirementSpecification");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
