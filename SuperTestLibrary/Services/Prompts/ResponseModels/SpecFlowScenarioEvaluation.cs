using SuperTestLibrary.Services.Prompts.ResponseModels.SpecFlowScenarioEvaluationCriteria;

namespace SuperTestLibrary.Services.Prompts.ResponseModels
{
    public class SpecFlowScenarioEvaluation : EvaluationMetric
    {
        public string ScenarioName { get; init; } = string.Empty;
        public ClarityAndReadabilityScenarioCriteria ClarityAndReadability { get; init; } = new();
        public StructureAndFocusScenarioCriteria StructureAndFocus { get; init; } = new();
        public MaintainabilityScenarioCriteria Maintainability { get; init; } = new();
        public TraceabilityScenarioCriteria Traceability { get; init; } = new();
        public EvaluationScore Score { get; set; } = new();
        public string Summary { get; init; } = string.Empty;

        public void AssignScore()
        {
            var propertiesToEvaluate = new Dictionary<string, int>
            {
                { nameof(ClarityAndReadability.HumanFriendlyLanguage), ClarityAndReadability.HumanFriendlyLanguage },
                { nameof(ClarityAndReadability.ConciseAndRelevantScenarios), ClarityAndReadability.ConciseAndRelevantScenarios },
                { nameof(ClarityAndReadability.LogicalFlow), ClarityAndReadability.LogicalFlow },
                { nameof(StructureAndFocus.FocusedScenario), StructureAndFocus.FocusedScenario },
                { nameof(StructureAndFocus.ScenarioStructure), StructureAndFocus.ScenarioStructure },
                { nameof(StructureAndFocus.ScenarioOutlines), StructureAndFocus.ScenarioOutlines },
                { nameof(Maintainability.MinimalCouplingToImplementation), Maintainability.MinimalCouplingToImplementation },
                { nameof(Maintainability.IndependentScenarios), Maintainability.IndependentScenarios },
                { nameof(Maintainability.TestDataManagement), Maintainability.TestDataManagement },
                { nameof(Traceability.TraceabilityToRequirements), Traceability.TraceabilityToRequirements }
            };

            Score = CalculateScore(propertiesToEvaluate);
        }
    }
}
