namespace AbcLeaves.BasicMvcClient.Helpers
{
    public interface IGoogleOAuthHelper
    {
        string BuildOfflineAccessChallengeUrl(string redirectUrl, string state);
    }
}
