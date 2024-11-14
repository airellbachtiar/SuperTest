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

            foreach (var property in propertiesToEvaluate)
            {
                if (property.Value == null)
                {
                    throw new InvalidOperationException($"The field '{property.Key}' has not been assigned.");
                }
                else if (property.Value > maxScorePerCategory)
                {
                    throw new InvalidOperationException($"The field '{property.Key}' has an invalid value.");
                }
            }

            var evaluatedProperties = propertiesToEvaluate.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value ?? 0
            );

            Score = CalculateScore(evaluatedProperties);
        }
    }
}
