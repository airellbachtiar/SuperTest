using SuperTestLibrary.Services.Prompts.ResponseModels;
using System.Text.Json;

namespace SuperTestLibrary.Helpers
{
    public static class GetSpecFlowFeatureFileEvaluation
    {
        public static EvaluateSpecFlowFeatureFileResponse ConvertJson(string response)
        {
            try
            {
                var specFlowFeatureFileEvaluation = JsonSerializer.Deserialize<EvaluateSpecFlowFeatureFileResponse>(response);

                if (specFlowFeatureFileEvaluation != null)
                {
                    specFlowFeatureFileEvaluation.CalculateScore();
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
