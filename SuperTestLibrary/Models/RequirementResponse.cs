using YamlDotNet.Serialization;

namespace SuperTestLibrary.Models
{
    public class RequirementResponse
    {
        public string Response { get; set; } = string.Empty;

        [YamlMember(Alias = "file_name")]
        public string FileName { get; set; } = string.Empty;
        [YamlMember(Alias = "title")]
        public string Title { get; set; } = string.Empty;
        [YamlMember(Alias = "requirements")]
        public List<RequirementModel> Requirements { get; set; } = [];

        public List<string> Prompts { get; set; } = [];
    }
}
