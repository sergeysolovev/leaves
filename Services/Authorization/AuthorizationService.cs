using System.Threading.Tasks;
using ABC.Leaves.Api.Authorization.Dto;
using ABC.Leaves.Api.GoogleAuth;
using ABC.Leaves.Api.Repositories;

namespace ABC.Leaves.Api.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IGoogleAuthService googleAuthService;

        public AuthorizationService(IEmployeeRepository employeeRepository,
            IGoogleAuthService googleAuthService)
        {
            this.employeeRepository = employeeRepository;
            this.googleAuthService = googleAuthService;
        }

        public async Task<AuthorizeAdminResult> AuthorizeAdmin(string accessToken)
        {
            var result = await googleAuthService.GetAccessTokenInfoAsync(accessToken);
            if (result.Error != null)
            {
                return new AuthorizeAdminResult { IsAuthorized = false, Error = result.Error };
            }
            var userEmail = result.Email;
            if (!employeeRepository.CheckUserIsAdmin(result.Email))
            {
                return new AuthorizeAdminResult { IsAuthorized = false };
            }
            return new AuthorizeAdminResult { IsAuthorized = true };
        }
    }
}
