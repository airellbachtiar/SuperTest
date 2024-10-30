
using System.Text;

namespace SuperTestLibrary.Services.Prompts.Builders
{
    public class EvaluateSpecFlowFeatureFilePromptBuilder : IPromptBuilder
    {
        private Prompt? prompt;
        private string? requirements;
        private string featureFile;

        public EvaluateSpecFlowFeatureFilePromptBuilder(string requirements, string featureFile)
        {
            if (string.IsNullOrEmpty(featureFile))
            {
                throw new ArgumentNullException(nameof(featureFile));
            }

            this.featureFile = featureFile;
            this.requirements = requirements;
        }

        public IEnumerable<string> BuildPrompt(Prompt prompt)
        {
            this.prompt = prompt;

            ArgumentNullException.ThrowIfNull(prompt);

            if (string.IsNullOrEmpty(requirements))
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            List<string> prompts = [BuildContext()];
            return prompts;
        }

        private string BuildContext()
        {
            StringBuilder promptBuilder = new();

            promptBuilder.AppendLine(prompt!.SystemInstruction);
            promptBuilder.AppendLine();

            promptBuilder.AppendLine("The scoring scale should be score as follows:");

            foreach (var score in prompt.ScoringScale)
            {
                promptBuilder.AppendLine(score);
            }

            promptBuilder.AppendLine();

            promptBuilder.AppendLine("Feature file evaluation has these following criteria:");

            foreach (var instruction in prompt.Criteria.Select((value, i) => new { i, value }))
            {
                promptBuilder.AppendLine($"{instruction.i + 1}. {instruction.value}");
            }

            promptBuilder.AppendLine();

            foreach (var instruction in prompt.Instructions)
            {
                promptBuilder.AppendLine(instruction);
            }

            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Feature file:");
            promptBuilder.AppendLine(featureFile);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Requirements:");
            promptBuilder.AppendLine(requirements);

            return promptBuilder.ToString();
        }
    }
}
