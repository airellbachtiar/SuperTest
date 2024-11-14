namespace SuperTestLibrary.Services.Prompts.ResponseModels
{
    public class EvaluationMetric
    {
        internal const int maxScorePerCategory = 5;

        internal EvaluationScore CalculateScore(Dictionary<string, int> criteria)
        {
            var score = new EvaluationScore();

            var validProperties = criteria
            .Where(prop => prop.Value >= 0)
                .ToList();

            score.TotalScore = validProperties.Sum(prop => prop.Value);
            score.MaximumScore = validProperties.Count * maxScorePerCategory;
            score.Percentage = score.MaximumScore > 0
            ? (double)score.TotalScore / score.MaximumScore * 100
            : 0;

            score.Percentage = Math.Round(score.Percentage, 2);

            return score;
        }
    }
}
