namespace SuperTestLibrary.Storages
{
    public interface IReqIFStorage
    {
        Task <IEnumerable<string>> GetAllReqIFsAsync();
    }
}
