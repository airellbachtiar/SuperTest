using SuperTestLibrary.Services;
using SuperTestLibrary.Storages;

namespace SuperTestLibrary
{
    public class SuperTestController : ISuperTestController
    {
        private readonly IReqIFStorage _reqIFStorage;

        public SuperTestController(IReqIFStorage reqIFStorage)
        {
            _reqIFStorage = reqIFStorage;
        }

        public async Task<string> GenerateSpecFlowFeatureFileAsync(string requirements)
        {
            Gemini1_5 gemini = new Gemini1_5();
            return await gemini.GenerateSpecFlowFeatureFileAsync(requirements);
        }

        public async Task<IEnumerable<string>> GetAllReqIFFilesAsync()
        {
            // TODO: Implement this method
            // For now, just mock reqif files
            return await _reqIFStorage.GetAllReqIFsAsync();
        }
    }
}
