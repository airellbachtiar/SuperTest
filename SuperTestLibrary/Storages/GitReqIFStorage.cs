
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
            try
            {
                return await Task.Run(() => Directory.GetFiles(_gitLocation, ReqIFExtensionFilter));
            }
            catch (Exception ex)
            {
                throw new DirectoryNotFoundException($"Directory not found. Unable to fetch ReqIF files from {_gitLocation}. {ex.Message}");
            }
        }

        public async Task<string> ReadReqIFFileAsync(string fileName, string directory)
        {
            try
            {
                return await Task.Run(() => File.ReadAllText(Path.Combine(directory, fileName)));
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException($"File not found. Unable to fetch ReqIF file {fileName} from {directory}. {ex.Message}");
            }
        }
    }
}
