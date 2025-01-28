using System.ComponentModel;

namespace LargeLanguageModelLibrary.Enums
{
    public enum ModelName
    {
        [Description("GPT-4o")]
        GPT4o,
        [Description("Claude 3.5 Sonnet")]
        Claude35Sonnet,
        [Description("Gemini 1.5")]
        Gemini15
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute?.Description ?? value.ToString();
        }
    }
}
