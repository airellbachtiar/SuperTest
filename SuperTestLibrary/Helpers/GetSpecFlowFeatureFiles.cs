using SuperTestLibrary.Services.Prompts;
using System.Text.Json;

namespace SuperTestLibrary.Helpers
{
    public static class GetSpecFlowFeatureFiles
    {
        public static SpecFlowFeatureFileResponse ConvertJson(string response)
        {
            var specFlowFeatureFiles = JsonSerializer.Deserialize<SpecFlowFeatureFileResponse>(response);

            if (specFlowFeatureFiles != null)
            {
                return specFlowFeatureFiles;
            }
            else return new SpecFlowFeatureFileResponse();
        }
    }
}
