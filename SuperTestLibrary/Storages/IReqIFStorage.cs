namespace SuperTestLibrary.Storages
{
    public interface IReqIFStorage
    {
        Task <IEnumerable<string>> GetAllReqIFsAsync();
        Task<string> FetchReqIFFileAsync(string fileName, string directory);
    }
}
