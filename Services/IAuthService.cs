namespace ABC.Leaves.Api.Services
{
    public interface IAuthService
    {
        string GetGoogleAuthenticationUrl(string redirectUrl);
        string GetAccessToken(string code, string redirectUrl);
    }
}
