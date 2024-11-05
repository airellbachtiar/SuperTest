using System.Collections.ObjectModel;

namespace SuperTestWPF.Models
{
    public class ScenarioModel
    {
        public string Name { get; set; } = string.Empty;
        public string Keyword { get; set; } = string.Empty;
        public bool IsAccepted { get; set; } = true;
        public ObservableCollection<StepModel> Steps { get; set; } = [];
    }
}
