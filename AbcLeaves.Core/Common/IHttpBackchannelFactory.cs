using System.Net.Http;

namespace AbcLeaves.Core
{
    public interface IHttpBackchannelFactory
    {
        IHttpBackchannel Create(HttpMessageHandler httpMessageHandler);
    }
}
