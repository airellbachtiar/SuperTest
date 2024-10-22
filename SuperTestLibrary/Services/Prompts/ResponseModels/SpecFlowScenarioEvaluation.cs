using SuperTestLibrary.Services.Prompts.ResponseModels.SpecFlowScenarioEvaluationCriteria;

namespace SuperTestLibrary.Services.Prompts.ResponseModels
{
    public class SpecFlowScenarioEvaluation
    {
        public string ScenarioID { get; init; } = string.Empty;
        public ClarityAndReadabilityScenarioCriteria ClarityAndReadability { get; init; } = new();
        public StructureAndFocusScenarioCriteria StructureAndFocus { get; init; } = new();
        public MaintainabilityScenarioCriteria Maintainability { get; init; } = new();
        public TraceabilityScenarioCriteria Traceability { get; init; } = new();
        public EvaluationScore Score { get; init; } = new();
        public string Summary { get; init; } = string.Empty;
    }
}
