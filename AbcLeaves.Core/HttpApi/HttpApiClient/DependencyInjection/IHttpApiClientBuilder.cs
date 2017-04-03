using System;
using Microsoft.Extensions.DependencyInjection;

namespace AbcLeaves.Core
{
    public interface IHttpApiClientBuilder
    {
        IServiceCollection Services { get; }
        IHttpApiClientBuilder Configure(IHttpApiOptions options);
        IHttpApiClientBuilder ConfigureOptions(Action<IHttpApiOptions> configureOptions);
        IHttpApiClientBuilder UseBackchannel<THttpBackchannel>()
            where THttpBackchannel : class, IHttpBackchannel;
        IHttpApiClientBuilder AddBearerTokenIdentification<TBearerTokenProvider>()
            where TBearerTokenProvider : class, IBearerTokenProvider;
    }
}
