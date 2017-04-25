using System;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpBackchannelServiceProviderExtensions
    {
        public static HttpMessageHandler GetBackchannelHttpHandler(this IServiceProvider provider)
        {
            return provider.GetService<HttpMessageHandler>();
        }
    }
}