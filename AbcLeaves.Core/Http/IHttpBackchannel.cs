using System.Net.Http;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public interface IHttpBackchannel
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
