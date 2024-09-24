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
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetAllReqIFFilesAsync()
        {
            // TODO: Implement this method
            // For now, just mock reqif files
            await Task.Delay(5000);
            return await _reqIFStorage.GetAllReqIFsAsync();
        }
    }
}
