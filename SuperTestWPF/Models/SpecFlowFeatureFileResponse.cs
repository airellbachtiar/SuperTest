namespace SuperTestWPF.Models
{
    public class SpecFlowFeatureFileResponse (IEnumerable<SpecFlowFeatureFileModel> featureFiles, IEnumerable<PromptHistory> prompts)
    {
        public IEnumerable<SpecFlowFeatureFileModel> FeatureFiles { get; set; } = featureFiles;
        public IEnumerable<PromptHistory> Prompts { get; set; } = prompts;
    }
}
