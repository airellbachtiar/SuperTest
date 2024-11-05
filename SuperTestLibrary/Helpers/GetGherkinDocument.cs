using Gherkin;
using Gherkin.Ast;
using SuperTestLibrary.Services.Prompts.ResponseModels;

namespace SuperTestLibrary.Helpers
{
    public static class GetGherkinDocument
    {
        public static GherkinDocument? ConvertSpecFlowFeatureFileResponse(SpecFlowFeatureFileResponse response)
        {
            var parser = new Parser();
            foreach (var featureFile in response.FeatureFiles)
            {
                var gherkinDocument = parser.Parse(new StringReader(featureFile.Value));

                // If no exception is thrown, the feature file is valid.
                if (gherkinDocument.Feature != null)
                {
                    return gherkinDocument;
                }
            }
            return null;
        }
    }
}
