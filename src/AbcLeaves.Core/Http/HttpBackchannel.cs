using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public class HttpBackchannel : IHttpBackchannel
    {
        private static DefaultHttpBackchannel localDefault;
        private readonly HttpMessageHandler httpMessageHandler;
        private HttpClient backchannel;

        internal class DefaultHttpBackchannel : HttpBackchannel
        {
            private DefaultHttpBackchannel() : base(new HttpClientHandler())
            {
            }

            internal static DefaultHttpBackchannel Create()
            {
                return new DefaultHttpBackchannel();
            }
        }

        public static IHttpBackchannel Default
        {
            get
            {
                localDefault = localDefault ?? DefaultHttpBackchannel.Create();
                return localDefault;
            }
        }

        public HttpBackchannel(HttpMessageHandler httpMessageHandler)
        {
            if (httpMessageHandler == null)
            {
                throw new ArgumentNullException(nameof(httpMessageHandler));
            }
            this.httpMessageHandler = httpMessageHandler;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            if (backchannel == null)
            {
                backchannel = new HttpClient(httpMessageHandler);
            }
            return backchannel.SendAsync(request);
        }
    }
}
