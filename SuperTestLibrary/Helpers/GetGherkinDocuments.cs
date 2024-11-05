using Gherkin;
using Gherkin.Ast;
using SuperTestLibrary.Services.Prompts.ResponseModels;

namespace SuperTestLibrary.Helpers
{
    public static class GetGherkinDocuments
    {
        public static List<GherkinDocument?> ConvertSpecFlowFeatureFileResponse(SpecFlowFeatureFileResponse response)
        {
            var parser = new Parser();
            var gherkinDocuments = new List<GherkinDocument?>();
            foreach (var featureFile in response.FeatureFiles)
            {
                var gherkinDocument = parser.Parse(new StringReader(featureFile.Value));

                // If no exception is thrown, the feature file is valid.
                if (gherkinDocument.Feature == null)
                {
                    gherkinDocuments.Add(null);
                }
                gherkinDocuments.Add(gherkinDocument);
            }
            return gherkinDocuments;
        }
    }
}
