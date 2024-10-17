using SuperTestLibrary.Helpers;
using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services;
using SuperTestLibrary.Services.Prompts;
using SuperTestLibrary.Storages;

namespace SuperTestLibrary
{
    public class SuperTestController : ISuperTestController
    {
        private readonly IReqIFStorage _reqIFStorage;
        private ILargeLanguageModel? _llm;
        private IGenerator? _specFlowFeatureFileGenerator;

        public SuperTestController(IReqIFStorage reqIFStorage)
        {
            _reqIFStorage = reqIFStorage;
        }

        public async Task<SpecFlowFeatureFileResponse> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            if (SelectedLLM == null)
            {
                throw new InvalidOperationException("No LLM has been set.");
            }

            if (SelectedGenerator == null)
            {
                throw new InvalidOperationException("No generator has been set.");
            }

            if(string.IsNullOrWhiteSpace(requirements))
            {
                throw new InvalidOperationException("No requirements provided.");
            }

            return await SelectedGenerator.Generate(SelectedLLM, requirements);
        }

        public async Task<IEnumerable<string>> GetAllReqIFFilesAsync()
        {
            return await _reqIFStorage.GetAllReqIFsAsync();
        }

        public IGenerator? SelectedGenerator { get; set; }
        public ILargeLanguageModel? SelectedLLM { get; set; }
    }

}
