namespace ABC.Leaves.Api.Services
{
    public interface IAuthenticationService
    {
        string GetGoogleAuthenticationUrl(string redirectUrl);
        string GetAccessToken(string code, string redirectUrl);
    }
}
