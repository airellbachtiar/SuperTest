﻿using SuperTestLibrary.Services.Prompts.ResponseModels;
using System.Text.Json;

namespace SuperTestLibrary.Helpers
{
    public static class GetSpecFlowScenarioEvaluation
    {
        public static EvaluateSpecFlowScenarioResponse ConvertJson(string response)
        {
            try
            {
                var specFlowScenarioEvaluation = JsonSerializer.Deserialize<EvaluateSpecFlowScenarioResponse>(response);

                if (specFlowScenarioEvaluation != null)
                {
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
