using SuperTestLibrary.Services.PromptBuilders;

namespace SuperTestLibrary.Helpers
{
    public static class GetEvaluateSpecFlowPromptBuilder
    {
        public static IEnumerable<string> SetupPrompt(string jsonPromptPath, string requirements, string featureFile)
        {
            var prompt = GetPromptFromJson.ConvertJson(jsonPromptPath);

            var prompts = new EvaluateSpecFlowFeatureFilePromptBuilder(requirements, featureFile).BuildPrompt(prompt);

            return prompts;
        }
    }
}
