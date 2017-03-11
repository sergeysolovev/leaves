namespace ABC.Leaves.Api.Services
{
    public class GoogleOAuthOptions
    {
        public GoogleOAuthOptions() { }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthUri { get; set; }
        public string TokenUri { get; set; }
        public string TokenInfoUri { get; set; }
        public string RefreshTokenUri { get; set; }
        public string[] Scopes { get; set; } = new string[] { };
    }
}