using System;
using Microsoft.Extensions.DependencyInjection;

namespace AbcLeaves.Core
{
    public class HttpApiClientBuilder : IHttpApiClientBuilder
    {
        private Action<IHttpApiOptions> configureOptions;
        public IServiceCollection Services { get; private set; }

        private HttpApiClientBuilder(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            Services = services;
        }

        public IHttpApiClientBuilder AddBearerTokenIdentification<TBearerTokenProvider>()
            where TBearerTokenProvider : class, IBearerTokenProvider
        {
            Services.AddSingleton<IBearerTokenProvider, TBearerTokenProvider>();
            Services.AddSingleton<IdentifyHttpRequestBearerFactory>();
            return this;
        }

        public IHttpApiClientBuilder UseBackchannel<THttpBackchannel>()
            where THttpBackchannel : class, IHttpBackchannel
        {
            var serviceProvider = Services.BuildServiceProvider();
            var backchannel = serviceProvider.GetRequiredService<THttpBackchannel>();
            return ConfigureOptions(opts => opts.Backchannel = backchannel);
        }

        public IHttpApiClientBuilder ConfigureOptions(
            Action<IHttpApiOptions> configureOptions)
        {
            this.configureOptions += configureOptions;
            return this;
        }

        public IHttpApiClientBuilder Configure(IHttpApiOptions options)
        {
            configureOptions.Invoke(options);
            return this;
        }

        public static HttpApiClientBuilder Create(IServiceCollection services)
        {
            return new HttpApiClientBuilder(services);
        }
    }
}
