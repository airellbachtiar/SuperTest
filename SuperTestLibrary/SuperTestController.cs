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

        public string GenerateSpecFlowFeatureFile()
        {
            // TODO: Implement this method
            return string.Empty;
        }

        public async Task<IEnumerable<string>> GetAllReqIFFilesAsync()
        {
            // TODO: Implement this method
            // For now, just mock reqif files
            return await _reqIFStorage.GetAllReqIFsAsync();
        }
    }
}
