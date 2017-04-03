namespace AbcLeaves.Core
{
    public class DefaultHttpApiOptions : IHttpApiOptions
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public HttpApiAuthType AuthType { get; set; }
        public IHttpBackchannel Backchannel { get; set; }
    }
}
