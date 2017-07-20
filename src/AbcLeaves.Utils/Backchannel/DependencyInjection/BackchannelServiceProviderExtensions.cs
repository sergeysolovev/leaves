using System;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BackchannelServiceProviderExtensions
    {
        public static HttpMessageHandler GetHttpMessageHandler(this IServiceProvider provider)
        {
            return provider.GetService<HttpMessageHandler>();
        }
    }
}
