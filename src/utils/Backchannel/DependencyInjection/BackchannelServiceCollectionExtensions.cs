using System.Net.Http;
using Leaves.Utils;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BackchannelServiceCollectionExtensions
    {
        public static IServiceCollection AddBackchannel(this IServiceCollection services)
        {
            services.TryAddSingleton<HttpMessageHandler, HttpClientHandler>();
            services.AddTransient<IBackchannelFactory, BackchannelFactory>();
            return services;
        }
    }
}
