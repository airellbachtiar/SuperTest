namespace SuperTestWPF.Models
{
    public class StepModel
    {
        public string Keyword { get; set; } // E.g., "Given", "When", "Then"
        public string Text { get; set; } // The actual step description

        public StepModel(string keyword, string text)
        {
            Keyword = keyword;
            Text = text;
        }
    }
}
