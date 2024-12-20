using SuperTestLibrary.Models;
using System.Text.Json;

namespace SuperTestLibrary.Helpers
{
    public class GetSpecFlowFeatureFileEvaluationResponse
    {
        public static EvaluateSpecFlowFeatureFileResponse ConvertJson(string response)
        {
            var specFlowFeatureFileEvaluation = JsonSerializer.Deserialize<EvaluateSpecFlowFeatureFileResponse>(response);

            if (specFlowFeatureFileEvaluation != null)
            {
                specFlowFeatureFileEvaluation.AssignScore();
                return specFlowFeatureFileEvaluation;
            }
            else return new EvaluateSpecFlowFeatureFileResponse();
        }
    }
}
