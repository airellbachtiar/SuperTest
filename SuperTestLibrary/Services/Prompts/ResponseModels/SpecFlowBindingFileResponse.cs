namespace SuperTestLibrary.Services.Prompts.ResponseModels
{
    public class SpecFlowBindingFileResponse
    {
        // Key: BindingFileName, Value: BindingFileContent
        public Dictionary<string, string> BindingFiles { get; init; } = [];
    }
}
