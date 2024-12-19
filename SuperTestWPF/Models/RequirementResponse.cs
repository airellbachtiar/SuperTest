namespace SuperTestWPF.Models
{
    public class RequirementResponse (string requirement, IEnumerable<RequirementModel> requirements, IEnumerable<PromptHistory> promptHistories)
    {
        public string Requirement { get; set; } = requirement;
        public IEnumerable<RequirementModel> Requirements { get; set; } = requirements;
        public IEnumerable<PromptHistory> Prompts { get; set; } = promptHistories;
    }
}
