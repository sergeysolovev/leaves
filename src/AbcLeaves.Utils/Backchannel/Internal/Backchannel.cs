using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace AbcLeaves.Utils
{
    internal class Backchannel : IBackchannel
    {
        private readonly string baseUrl;
        private readonly HttpClient client;

        public Backchannel(HttpMessageHandler httpMessageHandler, string baseUrl = null)
        {
            this.baseUrl = baseUrl;
            this.client = new HttpClient(
                Throw.IfNull(httpMessageHandler, nameof(httpMessageHandler))
            );
        }

        public async Task<SendMessageResult> SendAsync(string url, HttpMethod method,
            Action<IHttpRequestBuilder> configure = null)
        {
            var request = buildRequest();

            try
            {
                var response = await client.SendAsync(request);
                return SendMessageResult.Succeed(response);
            }
            catch (HttpRequestException error)
            {
                return SendMessageResult.Fail(error);
            }

            HttpRequestMessage buildRequest()
            {
                var builder = new HttpRequestBuilder(baseUrl);
                configure?.Invoke(builder);
                return builder.Build(url, method);
            }
        }
    }
}
