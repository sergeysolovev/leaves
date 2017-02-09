using ABC.Leaves.Api.Services.Dto;

namespace ABC.Leaves.Api.Authorization.Dto
{
    public class AuthorizeAdminResult
    {
        public bool IsAuthorized { get; set; }
        public ErrorDto Error { get; set; }
    }
}