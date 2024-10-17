using Gherkin;
using SuperTestLibrary.Services.Prompts;

namespace SuperTestLibrary.Helpers
{
    public static class ValidateFeatureFile
    {
        public static bool Validate(SpecFlowFeatureFileResponse response)
        {
            var parser = new Parser();
            try
            {
                foreach (var featureFile in response.FeatureFiles)
                {
                    var gherkinDocument = parser.Parse(new StringReader(featureFile.Value));

                    // If no exception is thrown, the feature file is valid.
                    if (gherkinDocument.Feature == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
