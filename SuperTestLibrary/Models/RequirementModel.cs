using YamlDotNet.Serialization;

namespace SuperTestLibrary.Models
{
    public class RequirementModel
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = string.Empty;
        [YamlMember(Alias = "content")]
        public string Content { get; set; } = string.Empty;
        [YamlMember(Alias = "trace")]
        public string Trace { get; set; } = string.Empty;
    }
}