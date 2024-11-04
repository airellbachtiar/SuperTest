
using SuperTestLibrary.Helpers;
using System.Text;

namespace SuperTestLibrary.Services.Prompts.Builders
{
    public class SpecFlowBindingFilePromptBuilder : PromptBuilderBase
    {
        private readonly string _featureFile;
        private readonly Dictionary<string, string> _generatedCSharpCode;

        public SpecFlowBindingFilePromptBuilder(string featureFile, Dictionary<string, string> generatedCSharpCode)
        {
            _featureFile = featureFile;
            _generatedCSharpCode = generatedCSharpCode;
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
            promptBuilder.AppendLine("I will provide you with SpecFlow feature file and some C# codes.");
            promptBuilder.AppendLine("Feature File:");
            promptBuilder.AppendLine(_featureFile);
            promptBuilder.AppendLine();

            if (_generatedCSharpCode.Any())
            {
                promptBuilder.AppendLine("C# Code:");
                foreach (var file in _generatedCSharpCode)
                {
                    promptBuilder.AppendLine($"// File: {file.Key}");
                    promptBuilder.AppendLine(file.Value);
                    promptBuilder.AppendLine("// END OF FILE");
                    promptBuilder.AppendLine();
                }
            }

            return promptBuilder.ToString();
        }
    }
}
