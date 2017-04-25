using Microsoft.Extensions.DependencyInjection;

namespace AbcLeaves.Core
{
    public interface IHttpApiClientBuilder
    {
        IServiceCollection Services { get; }

        IHttpApiClientBuilder UseBackchannel(IHttpBackchannel backchannel);

        IHttpApiClientBuilder AddBearerTokenIdentification<TBearerTokenProvider>()
            where TBearerTokenProvider : class, IBearerTokenProvider;
    }
}
