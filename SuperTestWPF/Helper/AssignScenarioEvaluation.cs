using SuperTestLibrary.Models;
using SuperTestWPF.Models;

namespace SuperTestWPF.Helper
{
    public static class AssignScenarioEvaluation
    {
        public static void Assign(string selectedLlmString, ScenarioModel scenarioModel, SpecFlowScenarioEvaluation scenario)
        {
            var score = scenario.Score;

            scenarioModel.ScenarioEvaluationScoreDetails.Add($"{selectedLlmString} Evaluation:");
            scenarioModel.ScenarioEvaluationScoreDetails.Add("--------------------------------------------------------------------------");
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"Scenario: {scenario.ScenarioName}");
            scenarioModel.ScenarioEvaluationScoreDetails.Add("Clarity and Readability");
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"\tHuman Friendly Language = {scenario.ClarityAndReadability.HumanFriendlyLanguage}/5 ");
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"\tConcise and Relevant Scenarios = {scenario.ClarityAndReadability.ConciseAndRelevantScenarios}/5 ");
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"\tLogical Flow = {scenario.ClarityAndReadability.LogicalFlow}/5 ");

            scenarioModel.ScenarioEvaluationScoreDetails.Add("Structure and Focus");
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"\tFocused Scenario = {scenario.StructureAndFocus.FocusedScenario}/5 ");
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"\tScenario Structure = {scenario.StructureAndFocus.ScenarioStructure}/5 ");
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"\tScenario Outlines = {scenario.StructureAndFocus.ScenarioOutlines}/5 ");

            scenarioModel.ScenarioEvaluationScoreDetails.Add("Maintainability");
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"\tMinimal Coupling to Implementation = {scenario.Maintainability.MinimalCouplingToImplementation}/5 ");
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"\tIndependent Scenarios = {scenario.Maintainability.IndependentScenarios}/5 ");
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"\tTest Data Management = {scenario.Maintainability.TestDataManagement}/5 ");

            scenarioModel.ScenarioEvaluationScoreDetails.Add("Traceability");
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"\tTraceability = {scenario.Traceability.TraceabilityToRequirements}/5 ");

            scenarioModel.ScenarioEvaluationScoreDetails.Add(string.Empty);
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"Total Score = {score.TotalScore}/{score.MaximumScore} ");
            scenarioModel.ScenarioEvaluationScoreDetails.Add($"Feature file score ({selectedLlmString}): {score.Percentage}% good");
            scenarioModel.ScenarioEvaluationScoreDetails.Add("--------------------------------------------------------------------------");

            scenarioModel.ScenarioEvaluationSummary += $"({selectedLlmString})Scenario: {scenario.ScenarioName}\n{scenario.Summary}\n";
        }
    }
}
