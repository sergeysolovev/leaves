using ABC.Leaves.Api.Services.Dto;

namespace ABC.Leaves.Api.GoogleAuth.Dto
{
    public class GetAccessTokenAsyncOutput
    {
        public string AccessToken { get; set; }
        public ErrorDto Error { get; set; }
    }
}
