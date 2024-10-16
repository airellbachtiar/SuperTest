namespace SuperTestLibrary.Storages
{
    public interface IReqIFStorage
    {
        Task <IEnumerable<string>> GetAllReqIFsAsync();
        Task<string> ReadReqIFFileAsync(string fileName, string directory);
    }
}
