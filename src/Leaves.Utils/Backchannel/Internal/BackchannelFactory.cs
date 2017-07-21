using System.Net.Http;

namespace Leaves.Utils
{
    internal class BackchannelFactory : IBackchannelFactory
    {
        private readonly HttpMessageHandler httpMessageHandler;

        public BackchannelFactory(HttpMessageHandler httpMessageHandler)
            => this.httpMessageHandler = Throw.IfNull(
                httpMessageHandler,
                nameof(httpMessageHandler)
            );

        public IBackchannel Create(string baseUrl = null)
            => new Backchannel(httpMessageHandler, baseUrl);
    }
}
