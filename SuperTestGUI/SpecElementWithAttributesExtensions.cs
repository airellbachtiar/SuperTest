using ReqIFSharp;
using HtmlAgilityPack;

namespace SuperTestGUI
{
    public static class SpecElementWithAttributesExtensions
    {
        public static object ExtractDisplayName(this SpecElementWithAttributes specElementWithAttributes)
        {
            if (string.IsNullOrEmpty(specElementWithAttributes.LongName))
            {
                var attributeValue = specElementWithAttributes.Values.FirstOrDefault();
                if (attributeValue != null)
                {
                    if (attributeValue is AttributeValueXHTML attributeValueXhtml)
                    {
                        return RemoveXhtmlTags(attributeValueXhtml.TheValue);
                    }
                    else
                    {
                        return attributeValue.ObjectValue?.ToString() ?? string.Empty;
                    }
                }
                else
                {
                    return specElementWithAttributes.Identifier;
                }
            }
            else
            {
                return specElementWithAttributes.LongName;
            }
        }

        public static string RemoveXhtmlTags(this string? xhtml)
        {
            if (string.IsNullOrEmpty(xhtml)) return string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(xhtml);
            return doc.DocumentNode.InnerText;
        }
    }
}
