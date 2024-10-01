using ReqIFSharp;
using SuperTestWPF.Helper;
using SuperTestWPF.Models;

namespace SuperTestWPF.Converters
{
    public class ReqIfUriToRequirementSpecificationConverter
    {
        public RequirementSpecification? Convert(string reqIfPath)
        {
            ReqIFDeserializer deserializer = new ReqIFDeserializer();
            ReqIF? reqIf = deserializer.Deserialize(reqIfPath).FirstOrDefault();

            if( reqIf == null)
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

            return new RequirementSpecification(header, requirements);
        }
    }
}
