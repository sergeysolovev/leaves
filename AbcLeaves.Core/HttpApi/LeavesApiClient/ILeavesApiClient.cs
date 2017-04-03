using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public interface ILeavesApiClient : IHttpApiClient
    {
        Task<CallHttpApiResult> ApplyLeaveAsync(CreateLeaveContract leave);
        Task<CallHttpApiResult> ApproveLeaveAsync(string id);
        Task<CallHttpApiResult> DeclineLeaveAsync(string id);
        Task<CallHttpApiResult> GrantGoogleApisAccess(string code, string redirectUrl);
        Task<VerifyAccessResult> VerifyGoogleApisAccess();
    }
}
