using System.Text;

namespace SuperTestLibrary.Services.Prompts.Builders
{
    public class SpecFlowFeatureFilePromptBuilder : PromptBuilderBase
    {
        private readonly string? _requirements;

        public SpecFlowFeatureFilePromptBuilder(string requirements)
        {
            _requirements = requirements;
        }

        protected override string BuildContext()
        {
            var promptBuilder = new StringBuilder();

            promptBuilder.AppendLine(_prompt!.SystemInstruction);
            promptBuilder.AppendLine();

            promptBuilder.AppendLine("Instructions:");

            foreach (var (instruction, index) in _prompt.Instructions.Select((value, i) => (value, i)))
            {
                promptBuilder.AppendLine($"{index + 1}. {instruction}");
            }

            promptBuilder.AppendLine();
            promptBuilder.AppendLine(_prompt.Thinking);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine(_prompt.Example);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Convert this requirements to a SpecFlow featureFile:");
            promptBuilder.AppendLine(_requirements);

            return promptBuilder.ToString();
        }
    }
}
