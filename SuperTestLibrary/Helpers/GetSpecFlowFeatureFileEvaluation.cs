using SuperTestLibrary.Services.Prompts.ResponseModels;
using System.Text.Json;

namespace SuperTestLibrary.Helpers
{
    public static class GetSpecFlowFeatureFileEvaluation
    {
        public static EvaluateSpecFlowFeatureFileResponse ConvertJson(string response)
        {
            var specFlowFeatureFileEvaluation = JsonSerializer.Deserialize<EvaluateSpecFlowFeatureFileResponse>(response);

            if (specFlowFeatureFileEvaluation != null)
            {
                return specFlowFeatureFileEvaluation;
            }
            else return new EvaluateSpecFlowFeatureFileResponse();
        }
    }
}
