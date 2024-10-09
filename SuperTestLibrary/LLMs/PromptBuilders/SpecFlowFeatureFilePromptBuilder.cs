using System.Text;

namespace SuperTestLibrary.LLMs.PromptBuilders
{
    public class SpecFlowFeatureFilePromptBuilder : IPromptBuilder
    {
        public string BuildPrompt(Prompt prompt, string requirements)
        {
            StringBuilder promptBuilder = new StringBuilder();

            promptBuilder.AppendLine(prompt.SystemInstruction);
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
    }
}
