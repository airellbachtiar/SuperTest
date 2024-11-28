namespace SuperTestLibrary.Services.PromptBuilders.ResponseModels.SpecFlowScenarioEvaluationCriteria
{
    public class MaintainabilityScenarioCriteria
    {
        public int? MinimalCouplingToImplementation { get; init; } = 0;
        public int? IndependentScenarios { get; init; } = 0;
        public int? TestDataManagement { get; init; } = 0;
    }
}