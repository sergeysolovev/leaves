using System.Threading.Tasks;
using ABC.Leaves.Api.Authorization.Dto;

namespace ABC.Leaves.Api.Authorization
{
    public interface IAuthorizationService
    {
        Task<AuthorizeAdminResult> AuthorizeAdminAsync(string accessToken);
    }
}
