namespace SuperTestLibrary.Services.Prompts.ResponseModels
{
    public class EvaluateSpecFlowScenarioResponse
    {
        public List<SpecFlowScenarioEvaluation> ScenarioEvaluations { get; init; } = [];

        public void AssignScores()
        {
            foreach (var scenarioEvaluation in ScenarioEvaluations)
            {
                scenarioEvaluation.AssignScore();
            }
        }
    }
}
