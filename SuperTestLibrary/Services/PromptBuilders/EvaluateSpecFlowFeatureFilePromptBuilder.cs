using System.Text;

namespace SuperTestLibrary.Services.PromptBuilders
{
    public class EvaluateSpecFlowFeatureFilePromptBuilder : PromptBuilderBase
    {
        private readonly string? _requirements;
        private readonly string _featureFile;

        public EvaluateSpecFlowFeatureFilePromptBuilder(string requirements, string featureFile)
        {
            ArgumentNullException.ThrowIfNull(requirements);
            ArgumentNullException.ThrowIfNull(featureFile);

            _featureFile = featureFile;
            _requirements = requirements;
        }

        protected override string BuildContext()
        {
            StringBuilder promptBuilder = new();

            promptBuilder.AppendLine(_prompt!.SystemInstruction);
            promptBuilder.AppendLine();

            promptBuilder.AppendLine("The scoring scale should be score as follows:");

            foreach (var score in _prompt.ScoringScale)
            {
                promptBuilder.AppendLine(score);
            }

            promptBuilder.AppendLine();

            promptBuilder.AppendLine("The evaluation has these following criteria:");

            foreach (var criteria in _prompt.Criteria)
            {
                promptBuilder.AppendLine(criteria);
            }

            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Instructions:");

            foreach (var instruction in _prompt.Instructions)
            {
                promptBuilder.AppendLine(instruction);
            }

            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Feature file:");
            promptBuilder.AppendLine(_featureFile);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Requirements:");
            promptBuilder.AppendLine(_requirements);

            return promptBuilder.ToString();
        }
    }
}
