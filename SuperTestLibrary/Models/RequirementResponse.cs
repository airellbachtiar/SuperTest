namespace SuperTestLibrary.Models
{
    public class RequirementResponse
    {
        public string Requirement { get; set; } = string.Empty;
        public List<RequirementModel> Requirements { get; set; } = [];
        public List<string> Prompts { get; set; } = [];
    }
}
