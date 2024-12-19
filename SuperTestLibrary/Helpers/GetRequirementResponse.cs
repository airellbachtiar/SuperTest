using SuperTestLibrary.Models;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace SuperTestLibrary.Helpers
{
    public static class GetRequirementResponse
    {
        public static IEnumerable<RequirementModel> ConvertYaml(string response)
        {
            var deserializer = new DeserializerBuilder().Build();

            var requirementResponse = deserializer.Deserialize<List<RequirementModel>>(response);
            if (requirementResponse != null)
            {
                return requirementResponse;
            }
            else return [];
        }
    }
}
