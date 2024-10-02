using SuperTestLibrary.LLMs;
using SuperTestLibrary.Storages;

namespace SuperTestLibrary
{
    public class SuperTestController : ISuperTestController
    {
        private readonly IReqIFStorage _reqIFStorage;
        private ILargeLanguageModel? _llm = new Gemini_1_5();

        public SuperTestController(IReqIFStorage reqIFStorage)
        {
            _reqIFStorage = reqIFStorage;
        }

        public async Task<string> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            return await _llm.GenerateSpecFlowFeatureFileAsync(requirements);
        }

        public async Task<IEnumerable<string>> GetAllReqIFFilesAsync()
        {
            // TODO: Implement this method
            // For now, just mock reqif files
            return await _reqIFStorage.GetAllReqIFsAsync();
        }

        public void SetLLM(ILargeLanguageModel llm)
        {
            _llm = llm;
        }
    }
}
