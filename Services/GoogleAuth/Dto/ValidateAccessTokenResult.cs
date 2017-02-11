using ABC.Leaves.Api.Services.Dto;

namespace ABC.Leaves.Api.GoogleAuth.Dto
{
    public class ValidateAccessTokenResult
    {
        public bool IsValid { get; set; }
        public ErrorDto Error { get; set; }
    }
}
