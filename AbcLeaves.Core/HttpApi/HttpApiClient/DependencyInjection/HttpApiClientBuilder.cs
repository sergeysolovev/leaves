using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AbcLeaves.Core
{
    public class HttpApiClientBuilder<TClient> : IHttpApiClientBuilder
        where TClient : class
    {
        public IServiceCollection Services { get; private set; }

        public HttpApiClientBuilder(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Services = services;
        }

        public IHttpApiClientBuilder UseBackchannel(IHttpBackchannel backchannel)
        {
            return Configure(opts => opts.Backchannel = backchannel);
        }

        public IHttpApiClientBuilder AddBearerTokenIdentification<TBearerTokenProvider>()
            where TBearerTokenProvider : class, IBearerTokenProvider
        {
            Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            Services.TryAddScoped<IBearerTokenProvider, TBearerTokenProvider>();
            Services.TryAddEnumerable(ServiceDescriptor
                .Scoped<IIdentifyHttpRequest, IdentifyHttpRequestBearer>());
            return this;
        }

        private HttpApiClientBuilder<TClient> Configure(
            Action<HttpApiClientOptions<TClient>> configureAction)
        {
            Services.Configure(configureAction);
            return this;
        }
    }
}
