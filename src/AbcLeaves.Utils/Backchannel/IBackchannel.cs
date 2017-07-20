using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AbcLeaves.Utils
{
    public interface IBackchannel
    {
        Task<SendMessageResult> SendAsync(
            string url,
            HttpMethod method,
            Action<IHttpRequestBuilder> configure = null
        );
    }
}
