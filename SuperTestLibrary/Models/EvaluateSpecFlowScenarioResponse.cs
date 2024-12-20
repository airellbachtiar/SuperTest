namespace SuperTestLibrary.Models
{
    public class EvaluateSpecFlowScenarioResponse
    {
        public List<SpecFlowScenarioEvaluation> ScenarioEvaluations { get; init; } = [];

        public List<string> Prompts { get; set; } = [];

        public void AssignScores()
        {
            foreach (var scenarioEvaluation in ScenarioEvaluations)
            {
                scenarioEvaluation.AssignScore();
            }
        }
    }
}
