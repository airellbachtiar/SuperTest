using System.Text;

namespace SuperTestLibrary.Services.Prompts.Builders
{
    public class SpecFlowFeatureFilePromptBuilder : IPromptBuilder
    {
        private Prompt? prompt;
        private string? requirements;

        public IEnumerable<string> BuildPrompt(Prompt prompt, string requirements)
        {
            this.prompt = prompt;
            this.requirements = requirements;

            if(prompt == null)
            {
                throw new ArgumentNullException(nameof(prompt));
            }

            List<string> prompts = new List<string>();
            prompts.Add(BuildContext());

            if (prompt.Instructions.Any())
            {
                prompts.AddRange(BuildInteractions());
            }

            return prompts;
        }

        private string BuildContext()
        {
            StringBuilder promptBuilder = new StringBuilder();

            promptBuilder.AppendLine(prompt!.SystemInstruction);
            promptBuilder.AppendLine();

            foreach (var instruction in prompt.Instructions.Select((value, i) => new { i, value }))
            {
                promptBuilder.AppendLine($"{instruction.i + 1}. {instruction.value}");
            }

            promptBuilder.AppendLine();
            promptBuilder.AppendLine(prompt.Thinking);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine(prompt.Example);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("<Requirements>");
            promptBuilder.AppendLine(requirements);
            promptBuilder.AppendLine("</Requirements>");

            return promptBuilder.ToString();
        }

        private IEnumerable<string> BuildInteractions()
        {
            List<string> interactions = new List<string>();

            foreach (var interaction in prompt!.Interactions)
            {
                interactions.Add(interaction.Message);
            }

            return interactions;
        }
    }
}
