namespace ABC.Leaves.Api.Services
{
    public class GoogleAuthOptions
    {
        public GoogleAuthOptions()
        {
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthUri { get; set; }
        public string TokenUri { get; set; }

    }
}