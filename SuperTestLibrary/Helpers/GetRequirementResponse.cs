using SuperTestLibrary.Models;
using YamlDotNet.Serialization;

namespace SuperTestLibrary.Helpers
{
    public static class GetRequirementResponse
    {
        public static RequirementResponse ConvertYaml(string response)
        {
            var deserializer = new DeserializerBuilder().Build();

            return deserializer.Deserialize<RequirementResponse>(response);
        }
    }
}
