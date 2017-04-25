namespace AbcLeaves.Core
{
    public class HttpApiClientOptions<THttpApiClient> : ICallHttpApiOptions
        where THttpApiClient : class
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public IHttpBackchannel Backchannel { get; set; }
    }
}
