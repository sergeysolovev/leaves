namespace AbcLeaves.BasicMvcClient.Domain
{
    public class GoogleOAuthOptions
    {
        public GoogleOAuthOptions() { }
        public string ClientId { get; set; }
        public string AuthUri { get; set; }
        public string[] Scopes { get; set; } = new string[] { };
    }
}