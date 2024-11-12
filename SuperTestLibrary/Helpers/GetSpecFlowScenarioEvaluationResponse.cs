using SuperTestLibrary.Services.Prompts.ResponseModels;
using System.Text.Json;

namespace SuperTestLibrary.Helpers
{
    public class GetSpecFlowScenarioEvaluationResponse
    {
        public static EvaluateSpecFlowScenarioResponse ConvertJson(string response)
        {
            try
            {
                var specFlowScenarioEvaluation = JsonSerializer.Deserialize<EvaluateSpecFlowScenarioResponse>(response);

                if (specFlowScenarioEvaluation != null)
                {
                    specFlowScenarioEvaluation.AssignScores();
                    return specFlowScenarioEvaluation;
                }
                else return new EvaluateSpecFlowScenarioResponse();
            }
            catch
            {
                throw;
            }
        }
    }
}
