namespace ABC.Leaves.Api.GoogleAuth
{
    public class GoogleAuthOptions
    {
        public GoogleAuthOptions()
        {
            Scopes = new string[] { };
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthUri { get; set; }
        public string TokenUri { get; set; }
        public string TokenInfoUri { get; set; }
        public string ResponseType { get; set; }
        public string[] Scopes { get; set; }

    }
}