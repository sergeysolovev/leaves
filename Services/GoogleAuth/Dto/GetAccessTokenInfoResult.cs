using ABC.Leaves.Api.Services.Dto;

namespace ABC.Leaves.Api.GoogleAuth.Dto
{
    public class GetAccessTokenInfoResult
    {
        public string Email { get; set; }
        public ErrorDto Error { get; set; }
    }
}
