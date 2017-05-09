using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Operations.Extensions.Http
{
    public abstract class GetHttpApiResponse :
        IOperationService<(HttpRequestMessage request, HttpClient client), IHttpApiResult>
    {
        // private IHttpBackchannel backchannel;
        // private HttpMessageHandler BackchannelHttpHandler;

        // protected GetHttpApiResponse(HttpMessageHandler backchannelHttpHandler)
        // {
        // }

        // // protected GetHttpApiResponse(IHttpBackchannel backchannel)
        // //     => this.backchannel = Throw.IfNull(backchannel, nameof(backchannel));

        // protected abstract Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        // // Backchannel = new HttpClient(Options.BackchannelHttpHandler ?? new HttpClientHandler());
        // // Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core OAuth middleware");
        // // Backchannel.Timeout = Options.BackchannelTimeout;
        // // Backchannel.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB

        // why not to pass HttpClient cz

        public IOperation<IHttpApiResult> Inject(
            (HttpRequestMessage request, HttpClient client) source)
        {
            return Operation.Return(sendAsync);

            async Task<IContext<IHttpApiResult>> sendAsync()
            {
                var response = await source.client.SendAsync(source.request);
                return Context.Succeed(new HttpApiResult(null, null));
            }
        }
    }
}