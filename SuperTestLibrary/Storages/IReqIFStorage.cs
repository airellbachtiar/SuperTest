using ReqIFSharp;

namespace SuperTestLibrary.Storages
{
    public interface IReqIFStorage
    {
        Task <List<ReqIF>> GetAllReqIFsAsync();
    }
}
