namespace AbcLeaves.BasicMvcClient.Helpers
{
    public class GoogleOAuthOptions
    {
        public GoogleOAuthOptions() { }
        public string ClientId { get; set; }
        public string AuthUri { get; set; }
        public string[] Scopes { get; set; } = new string[] { };
    }
}
