using System;
using System.Net.Http;
using AbcLeaves.Core;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddBackchannel<THttpMessageHandler>
        (
            this IServiceCollection services
        )
            where THttpMessageHandler : HttpMessageHandler
        {
            services.AddSingleton<HttpMessageHandler, THttpMessageHandler>();
            services.AddSingleton<HttpBackchannelFactory>();
            services.Add(ServiceDescriptor.Singleton(
                serviceType: typeof(HttpBackchannel),
                implementationFactory: serviceProvider => {
                    var factory = serviceProvider.GetRequiredService<HttpBackchannelFactory>();
                    var handler = serviceProvider.GetRequiredService<HttpMessageHandler>();
                    return factory.Create(handler);
                }));
            return services;
        }

        public static IHttpApiClientBuilder AddHttpApiClient<
            THttpApiClient, THttpApiClientFactory>
        (
            this IServiceCollection services,
            IConfiguration configuration
        )
            where THttpApiClient : class, IHttpApiClient
            where THttpApiClientFactory : class, IHttpApiClientFactory
        {
            return services.AddHttpApiClient<THttpApiClient, THttpApiClientFactory>(
                options => configuration.Bind(options));
        }

        public static IHttpApiClientBuilder AddHttpApiClient<
            THttpApiClient, THttpApiClientFactory>
        (
            this IServiceCollection services,
            Action<IHttpApiOptions> configureOptions
        )
            where THttpApiClient : class, IHttpApiClient
            where THttpApiClientFactory : class, IHttpApiClientFactory
        {
            var builder = HttpApiClientBuilder
                .Create(services)
                .ConfigureOptions(configureOptions);
            services.AddSingleton<HttpMessageHandler, HttpClientHandler>();
            services.AddSingleton<IHttpBackchannel, HttpBackchannel>();
            services.AddTransient<ICallHttpApiFactory, CallHttpApiFactory>();
            services.AddTransient<ICallHttpApiBuilderFactory, CallHttpApiBuilderFactory>();
            services.AddTransient<IHttpApiClientServiceFactory, HttpApiClientServiceFactory>();
            services.AddTransient<DefaultHttpApiOptions>();
            services.AddSingleton<THttpApiClientFactory>();
            services.AddSingleton<
                IIdentifyHttpApiRequestFactory,
                IdentifyHttpApiRequestFactory>
            (
                serviceProvider => {
                    var identifyApiRequestFactory = IdentifyHttpApiRequestFactory.Create();
                    RegisterIdentifyHttpRequestFactory<IdentifyHttpRequestBearerFactory>(
                        serviceProvider, identifyApiRequestFactory);
                    return identifyApiRequestFactory;
                }
            );
            services.Add(ServiceDescriptor.Singleton(
                serviceType: typeof(THttpApiClient),
                implementationFactory: serviceProvider => {
                    var options = serviceProvider.GetRequiredService<DefaultHttpApiOptions>();
                    var factory = serviceProvider.GetRequiredService<THttpApiClientFactory>();
                    var backchannel = serviceProvider.GetService<HttpBackchannel>();
                    if (backchannel != null)
                    {
                        builder.ConfigureOptions(opts => opts.Backchannel = backchannel);
                    }
                    builder.Configure(options);
                    return factory.Create(options);
                }));
            return HttpApiClientBuilder.Create(services);
        }

        private static void RegisterIdentifyHttpRequestFactory<T>(
            IServiceProvider serviceProvider,
            IIdentifyHttpApiRequestFactory identifyApiRequestFactory
        )
            where T : IIdentifyHttpRequestFactory
        {
            var identifyRequestFactory = serviceProvider.GetService<T>();
            if (identifyRequestFactory != null)
            {
                var authType = identifyRequestFactory.AuthType;
                if (!identifyApiRequestFactory.IsRegisteredAuthType(authType))
                {
                    identifyApiRequestFactory.RegisterIdentifyHttpRequestFactory(
                        identifyRequestFactory);
                }
            }
        }
    }
}