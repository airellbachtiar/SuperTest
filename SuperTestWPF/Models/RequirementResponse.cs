namespace SuperTestWPF.Models
{
    public class RequirementResponse (string requirement, IEnumerable<PromptHistory> promptHistories)
    {
        public string Requirement { get; set; } = requirement;
        public IEnumerable<PromptHistory> Prompts { get; set; } = promptHistories;
    }
}
