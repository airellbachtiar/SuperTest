using System.Collections.ObjectModel;

namespace SuperTestWPF.Models
{
    public class ScenarioModel
    {
        public string Name { get; set; } = string.Empty;
        public string Keyword { get; set; } = string.Empty;
        public ObservableCollection<TagModel> Tags { get; set; } = [];
        public bool IsAccepted { get; set; } = true;
        public bool IsSelected { get; set; } = false;
        public ObservableCollection<StepModel> Steps { get; set; } = [];
        public ObservableCollection<string> ScenarioEvaluationScoreDetails { get; set; } = [];
        public string ScenarioEvaluationSummary { get; set; } = string.Empty;
    }
}
