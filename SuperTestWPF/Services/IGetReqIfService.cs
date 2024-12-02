
namespace SuperTestWPF.Services
{
    public interface IGetReqIfService
    {
        Task<IEnumerable<string>> GetAll();
    }
}