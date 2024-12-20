namespace SuperTestWPF.Models
{
    public class PromptHistory (DateTime dateTime, string function, string largeLanguageModel, string prompt)
    {
        public DateTime DateTime { get; set; } = dateTime;
        public string Function { get; set; } = function;
        public string LargeLanguageModel { get; set; } = largeLanguageModel;
        public string Prompt { get; set; } = prompt;
    }
}
