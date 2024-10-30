using System.Text;

namespace SuperTestLibrary.Services.Prompts.Builders
{
    public class SpecFlowFeatureFilePromptBuilder : PromptBuilderBase, IPromptBuilder
    {
        private Prompt? prompt;
        private string? requirements;

        public SpecFlowFeatureFilePromptBuilder(string requirements)
        {
            this.requirements = requirements;
        }

        public IEnumerable<string> BuildPrompt(Prompt prompt)
        {
            this.prompt = prompt;

            ArgumentNullException.ThrowIfNull(prompt);

            List<string> prompts = [BuildContext()];

            if (prompt.Instructions.Any())
            {
                prompts.AddRange(BuildInteractions(prompt));
            }

            return prompts;
        }

        private string BuildContext()
        {
            StringBuilder promptBuilder = new();

            promptBuilder.AppendLine(prompt!.SystemInstruction);
            promptBuilder.AppendLine();

            promptBuilder.AppendLine("Instructions:");

            foreach (var instruction in prompt.Instructions.Select((value, i) => new { i, value }))
            {
                promptBuilder.AppendLine($"{instruction.i + 1}. {instruction.value}");
            }

            promptBuilder.AppendLine();
            promptBuilder.AppendLine(prompt.Thinking);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine(prompt.Example);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Convert this requirements to a SpecFlow featureFile:");
            promptBuilder.AppendLine(requirements);

            return promptBuilder.ToString();
        }
    }
}
