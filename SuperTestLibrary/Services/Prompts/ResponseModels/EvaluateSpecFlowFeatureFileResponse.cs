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

        private const int maxScorePerCategory = 5;

        public void CalculateScore()
        {
            var propertiesToEvaluate = new Dictionary<string, int>
            {
                { nameof(Readability), Readability },
                { nameof(Consistency), Consistency },
                { nameof(Focus), Focus },
                { nameof(Structure), Structure },
                { nameof(Maintainability), Maintainability },
                { nameof(Coverage), Coverage }
            };

            var validProperties = propertiesToEvaluate
                .Where(prop => prop.Value >= 0)
                .ToList();

            Score.TotalScore = validProperties.Sum(prop => prop.Value);

            Score.MaximumScore = validProperties.Count * maxScorePerCategory;

            Score.Percentage = Score.MaximumScore > 0
                ? (double)Score.TotalScore / Score.MaximumScore * 100
                : 0;

            Score.Percentage = Math.Round(Score.Percentage, 2);
        }
    }
}
