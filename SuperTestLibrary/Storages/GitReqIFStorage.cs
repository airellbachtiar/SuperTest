namespace SuperTestLibrary.Storages
{
    public class GitReqIFStorage : IReqIFStorage
    {
        private readonly string _gitLocation;

        private const string ReqIFExtension = ".reqif";
        private const string ReqIFExtensionFilter = "*" + ReqIFExtension;

        public GitReqIFStorage(string gitLocation)
        {
            _gitLocation = gitLocation;
        }

        public async Task<IEnumerable<string>> GetAllReqIFsAsync()
        {
            //TODO: Implement this method
            //For now, mock a list of ReqIF files
            return await Task.Run(() => Directory.GetFiles(_gitLocation, ReqIFExtensionFilter));
        }
    }
}
