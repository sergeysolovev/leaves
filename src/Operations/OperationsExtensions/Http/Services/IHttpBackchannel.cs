using System.Net.Http;
using System.Threading.Tasks;

namespace Operations.Extensions.Http
{
    public interface IHttpBackchannel
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}