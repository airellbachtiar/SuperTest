using ReqIFSharp;
using SuperTestWPF.Helper;
using SuperTestWPF.Models;
using System.Collections.ObjectModel;

namespace SuperTestWPF.Converters
{
    public class ReqIfUriToRequirementSpecificationConverter
    {
        private readonly ReqIFDeserializer _reqIfDeserializer = new ReqIFDeserializer();

        public RequirementSpecification? Convert(string reqIfPath)
        {
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

                return new RequirementSpecification(header, observableRequirements);
            }
            catch
            {
                throw new Exception("Error while converting ReqIF to RequirementSpecification");
            }
        }

        private bool IsReqIFContainContents(ReqIF reqIf)
        {
            return reqIf.TheHeader != null && reqIf.CoreContent != null;
        }
    }
}
