using System.Net.Http;
using AbcLeaves.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpBackchannelServiceCollectionExtensions
    {
        public static IServiceCollection AddBackchannel<THttpMessageHandler>(
            this IServiceCollection services
        )
            where THttpMessageHandler : HttpMessageHandler
        {
            services.TryAddSingleton<HttpMessageHandler, THttpMessageHandler>();
            services.TryAddSingleton<IHttpBackchannel, HttpBackchannel>();
            return services;
        }
    }
}