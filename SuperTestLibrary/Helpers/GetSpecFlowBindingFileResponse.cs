using SuperTestLibrary.Services.Prompts.ResponseModels;
using System.Text.Json;

namespace SuperTestLibrary.Helpers
{
    public static class GetSpecFlowBindingFileResponse
    {
        public static SpecFlowBindingFileResponse ConvertJson(string response)
        {
            var specFlowBindingFileResponse = JsonSerializer.Deserialize<SpecFlowBindingFileResponse>(response);

            if (specFlowBindingFileResponse != null)
            {
                return specFlowBindingFileResponse;
            }
            else return new SpecFlowBindingFileResponse();
        }
    }
}
