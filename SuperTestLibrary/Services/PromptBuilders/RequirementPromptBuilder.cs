using System.Text;

namespace SuperTestLibrary.Services.PromptBuilders
{
    public class RequirementPromptBuilder(Dictionary<string, string> testFiles, string existingRequirement) : PromptBuilderBase
    {
        protected override string BuildContext()
        {
            StringBuilder promptBuilder = new();

            promptBuilder.AppendLine(_prompt!.SystemInstruction);
            promptBuilder.AppendLine();

            promptBuilder.AppendLine("Instructions:");
            foreach (var (instruction, index) in _prompt.Instructions.Select((value, i) => (value, i)))
            {
                promptBuilder.AppendLine($"{index + 1}. {instruction}");
            }

            promptBuilder.AppendLine();
            promptBuilder.AppendLine(_prompt.Thinking);

            if (!string.IsNullOrEmpty(existingRequirement))
            {
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("Existing Requirement:");
                promptBuilder.AppendLine(existingRequirement);
            }

            if (testFiles.Count != 0)
            {
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("Test Files:");
                foreach (var file in testFiles)
                {
                    promptBuilder.AppendLine($"<FILE: {file.Key} />");
                    promptBuilder.AppendLine(file.Value);
                    promptBuilder.AppendLine("<END OF FILE />");
                    promptBuilder.AppendLine();
                }
            }

            return promptBuilder.ToString();
        }
    }
}
