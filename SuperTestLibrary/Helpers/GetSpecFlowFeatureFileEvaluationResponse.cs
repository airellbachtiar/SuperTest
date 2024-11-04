using SuperTestLibrary.Services.Prompts.ResponseModels;
using System.Text.Json;

namespace SuperTestLibrary.Helpers
{
    public class GetSpecFlowFeatureFileEvaluationResponse
    {
        public static EvaluateSpecFlowFeatureFileResponse ConvertJson(string response)
        {
            try
            {
                var specFlowFeatureFileEvaluation = JsonSerializer.Deserialize<EvaluateSpecFlowFeatureFileResponse>(response);

                if (specFlowFeatureFileEvaluation != null)
                {
                    specFlowFeatureFileEvaluation.AssignScore();
                    return specFlowFeatureFileEvaluation;
                }
                else return new EvaluateSpecFlowFeatureFileResponse();
            }
            catch
            {
                throw;
            }
        }
    }
}
