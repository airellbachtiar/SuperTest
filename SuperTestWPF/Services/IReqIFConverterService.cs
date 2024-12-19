using ReqIFSharp;
using SuperTestWPF.Models;

namespace SuperTestWPF.Services
{
    public interface IReqIFConverterService
    {
        ReqIF ConvertRequirementToReqIfAsync(IEnumerable<RequirementModel> requirements);
    }
}