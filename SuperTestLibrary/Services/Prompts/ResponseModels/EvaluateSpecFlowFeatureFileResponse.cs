namespace SuperTestLibrary.Services.Prompts.ResponseModels
{
    public class EvaluateSpecFlowFeatureFileResponse
    {
        public int Readability { get; init; }
        public int Consistency { get; init; }
        public int Focus { get; init; }
        public int Structure { get; init; }
        public int Maintainability { get; init; }
        public int Coverage { get; init; }
        public EvaluationScore Score { get; init; } = new();
        public string Summary { get; init; } = string.Empty;
    }
}
