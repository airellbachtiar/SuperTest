using SuperTestLibrary.Services.PromptBuilders.ResponseModels;
using SuperTestWPF.Models;

namespace SuperTestWPF.Helper
{
    public static class AssignSpecFlowFeatureFileEvaluation
    {
        public static void Assign(string selectedLlmString, SpecFlowFeatureFileModel featureFile, EvaluateSpecFlowFeatureFileResponse evaluationResponse)
        {
            var score = evaluationResponse.Score;

            featureFile.FeatureFileEvaluationScoreDetails.Add("=========================================================================");
            featureFile.FeatureFileEvaluationScoreDetails.Add($"{selectedLlmString} Evaluation:");
            featureFile.FeatureFileEvaluationScoreDetails.Add($"Readability = {evaluationResponse.Readability}/5 ");
            featureFile.FeatureFileEvaluationScoreDetails.Add($"Consistency = {evaluationResponse.Consistency}/5 ");
            featureFile.FeatureFileEvaluationScoreDetails.Add($"Focus = {evaluationResponse.Focus}/5 ");
            featureFile.FeatureFileEvaluationScoreDetails.Add($"Structure = {evaluationResponse.Structure}/5 ");
            featureFile.FeatureFileEvaluationScoreDetails.Add($"Maintainability = {evaluationResponse.Maintainability}/5 ");
            featureFile.FeatureFileEvaluationScoreDetails.Add($"Coverage = {evaluationResponse.Coverage}/5 ");
            featureFile.FeatureFileEvaluationScoreDetails.Add(string.Empty);
            featureFile.FeatureFileEvaluationScoreDetails.Add($"Total Score = {score.TotalScore}/{score.MaximumScore} ");
            featureFile.FeatureFileEvaluationScoreDetails.Add($"Feature file score ({selectedLlmString}): {score.Percentage}% good");
            featureFile.FeatureFileEvaluationScoreDetails.Add("=========================================================================");

            featureFile.FeatureFileEvaluationSummary += $"Evaluation from {selectedLlmString}:\n{evaluationResponse.Summary}\n";
        }
    }
}
