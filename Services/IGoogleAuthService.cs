namespace ABC.Leaves.Api.Services
{
    public interface IGoogleAuthService
    {
        string GetAuthUrl(string redirectUrl);
        string GetAccessToken(string code, string redirectUrl);
    }
}
