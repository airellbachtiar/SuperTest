namespace SuperTestLibrary.Services.PromptBuilders.ResponseModels.SpecFlowScenarioEvaluationCriteria
{
    public class StructureAndFocusScenarioCriteria
    {
        public int? FocusedScenario { get; init; } = 0;
        public int? ScenarioStructure { get; init; } = 0;
        public int? ScenarioOutlines { get; init; } = 0;
    }
}