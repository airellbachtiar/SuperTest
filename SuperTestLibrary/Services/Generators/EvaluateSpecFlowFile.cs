using SuperTestLibrary.Helpers;
using SuperTestLibrary.Services.Prompts.Builders;

namespace SuperTestLibrary.Services.Generators
{
    public class EvaluateSpecFlowFile
    {
        internal IEnumerable<string> SetupPrompt(string jsonPromptPath, string requirements, string featureFile)
        {
            var prompt = GetPrompt.ConvertJson(jsonPromptPath);

            var prompts = new EvaluateSpecFlowFeatureFilePromptBuilder(requirements, featureFile).BuildPrompt(prompt);

            return prompts;
        }
    }
}
