namespace SuperTestWPF.Models
{
    public class SpecFlowBindingFileResponse (IEnumerable<SpecFlowBindingFileModel> specFlowBindingFileModels, IEnumerable<PromptHistory> promptHistories)
    {
        public IEnumerable<SpecFlowBindingFileModel> specFlowBindingFileModels { get; set; } = specFlowBindingFileModels;
        public IEnumerable<PromptHistory> Prompts { get; set; } = promptHistories;
    }
}
