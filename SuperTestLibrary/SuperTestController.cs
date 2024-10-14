using SuperTestLibrary.LLMs;
using SuperTestLibrary.Services;
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

        public async Task<string> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            if (_llm == null)
            {
                throw new InvalidOperationException("No LLM has been set.");
            }

            if (_specFlowFeatureFileGenerator == null)
            {
                throw new InvalidOperationException("No generator has been set.");
            }

            return await _specFlowFeatureFileGenerator.Generate(_llm, requirements);
        }

        public async Task<IEnumerable<string>> GetAllReqIFFilesAsync()
        {
            return await _reqIFStorage.GetAllReqIFsAsync();
        }

        public void SetLLM(ILargeLanguageModel llm)
        {
            _llm = llm;
        }

        public void SetGenerator(IGenerator generator)
        {
            _specFlowFeatureFileGenerator = generator;
        }
    }

}
