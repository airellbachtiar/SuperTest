using SuperTestLibrary;

namespace SuperTestWPF.Services
{
    public class GetReqIfService : IGetReqIfService
    {
        private readonly ISuperTestController _controller;

        public GetReqIfService(ISuperTestController controller)
        {
            _controller = controller;
        }

        public async Task<IEnumerable<string>> GetAll()
        {
            return await _controller.GetAllReqIFFilesAsync();
        }

        public string RequirementsStorageLocation
        {
            get => _controller.GetStorageLocation();
            set => _controller.UpdateStorageLocation(value);
        }
    }
}
