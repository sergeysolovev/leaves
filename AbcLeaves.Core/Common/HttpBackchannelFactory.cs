using System.Net.Http;

namespace AbcLeaves.Core
{
    public class HttpBackchannelFactory : IHttpBackchannelFactory
    {
        public IHttpBackchannel Create(HttpMessageHandler httpMessageHandler)
        {
            return HttpBackchannel.Create(httpMessageHandler);
        }
    }
}
