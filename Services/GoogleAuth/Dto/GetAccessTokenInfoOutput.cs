using ABC.Leaves.Api.Services.Dto;

namespace ABC.Leaves.Api.GoogleAuth.Dto
{
    public class GetAccessTokenInfoOutput
    {
        public string Email { get; set; }
        public ErrorDto Error { get; set; }
    }
}
