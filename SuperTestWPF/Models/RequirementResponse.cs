namespace SuperTestWPF.Models
{
    public class RequirementResponse (string response, IEnumerable<RequirementModel> requirements, IEnumerable<PromptHistory> promptHistories)
    {
        public string Response { get; set; } = response;
        public string FileName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public IEnumerable<RequirementModel> Requirements { get; set; } = requirements;
        public IEnumerable<PromptHistory> Prompts { get; set; } = promptHistories;
    }
}
