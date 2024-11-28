namespace SuperTestLibrary.Services.PromptBuilders.ResponseModels
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

        internal Dictionary<string, int> CheckUnassignedValue(Dictionary<string, int?> propertiesToEvaluate)
        {
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

            return evaluatedProperties;
        }
    }
}
