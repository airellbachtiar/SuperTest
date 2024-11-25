using Gherkin.Ast;
using System.Collections.ObjectModel;

namespace SuperTestWPF.Models
{
    public class SpecFlowFeatureFileModel
    {
        public string FeatureFileName { get; set; } = string.Empty;
        public string FeatureFileContent { get; set; } = string.Empty;
        public string FeatureFileTitle { get; set; } = string.Empty;
        public ObservableCollection<string> FeatureFileEvaluationScoreDetails { get; set; } = [];
        public string FeatureFileEvaluationSummary { get; set; } = string.Empty;

        public GherkinDocument? GherkinDocument { get; set; } = null;
        public ObservableCollection<ScenarioModel> Scenarios { get; set; } = [];

        public SpecFlowFeatureFileModel(string featureFileName, string featureFileContent)
        {
            FeatureFileName = featureFileName;
            FeatureFileContent = featureFileContent;
        }

        public SpecFlowFeatureFileModel() { }

        public override string ToString()
        {
            return FeatureFileName;
        }
    }
}
