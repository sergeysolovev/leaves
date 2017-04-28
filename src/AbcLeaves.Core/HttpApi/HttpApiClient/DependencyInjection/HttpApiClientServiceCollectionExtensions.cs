using System;
using System.Net.Http;
using AbcLeaves.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpApiClientServiceCollectionExtensions
    {
        public static IHttpApiClientBuilder AddHttpApiClient<TClient>(
            this IServiceCollection services,
            IConfiguration configuration
        )
            where TClient : class
        {
            return services.AddHttpApiClient<TClient>(opts => configuration.Bind(opts));
        }

        public static IHttpApiClientBuilder AddHttpApiClient<TClient>(
            this IServiceCollection services,
            Action<HttpApiClientOptions<TClient>> configureOptions
        )
            where TClient : class
        {
            services.Configure(configureOptions);
            services.TryAddScoped<TClient>();
            services.TryAddScoped<IdentifyHttpRequestComposite>();
            services.TryAddScoped<ICallHttpApiBuilderFactory, DefaultCallHttpApiBuilderFactory>();
            services.TryAddScoped<IHttpApiClientServiceFactory, HttpApiClientServiceFactory>();
            return new HttpApiClientBuilder<TClient>(services);
        }
    }
}