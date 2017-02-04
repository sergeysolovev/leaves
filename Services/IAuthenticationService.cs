namespace ABC.Leaves.Api.Services
{
    public interface IAuthenticationService
    {
        string GetGoogleAuthenticationUrl();
        string GetAccessToken(string code);
    }
}
