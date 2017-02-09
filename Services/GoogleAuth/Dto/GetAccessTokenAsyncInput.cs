namespace ABC.Leaves.Api.GoogleAuth.Dto
{
    public class GetAccessTokenAsyncInput
    {
        public string Code { get; set; }
        public string RedirectUrl { get; set; }
    }
}
