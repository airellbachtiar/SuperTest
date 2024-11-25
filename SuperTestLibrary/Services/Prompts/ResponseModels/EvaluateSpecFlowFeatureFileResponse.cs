namespace SuperTestLibrary.Services.Prompts.ResponseModels
{
    public class EvaluateSpecFlowFeatureFileResponse : EvaluationMetric
    {
        public int? Readability { get; init; }
        public int? Consistency { get; init; }
        public int? Focus { get; init; }
        public int? Structure { get; init; }
        public int? Maintainability { get; init; }
        public int? Coverage { get; init; }
        public EvaluationScore Score { get; set; } = new();
        public string? Summary { get; init; } = string.Empty;

        public void AssignScore()
        {
            var propertiesToEvaluate = new Dictionary<string, int?>
            {
                { nameof(Readability), Readability },
                { nameof(Consistency), Consistency },
                { nameof(Focus), Focus },
                { nameof(Structure), Structure },
                { nameof(Maintainability), Maintainability },
                { nameof(Coverage), Coverage }
            };

            var evaluatedProperties = CheckUnassignedValue(propertiesToEvaluate);

            Score = CalculateScore(evaluatedProperties);
        }
    }
}
