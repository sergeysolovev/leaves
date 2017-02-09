namespace ABC.Leaves.Api.GoogleAuth.Dto
{
    public class GetAccessTokenInput
    {
        public string Code { get; set; }
        public string RedirectUrl { get; set; }
    }
}
