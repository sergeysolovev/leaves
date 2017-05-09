using System;
using System.Net.Http;
using System.Threading.Tasks;
using Operations;
using Operations.Extensions.Http;

namespace Operations.Exe
{
    public class CustomHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            System.Threading.CancellationToken cancellationToken)
        {
            IRequestBuilder requestBuilder = new RequestBuilder(request);
            var req = requestBuilder
                .UseAbsoluteUri("www.ya.ru", HttpMethod.Get)
                .WithBearerToken("abc")
                .Build()
                .ExecuteAsync()
                .Result
                .Result;
            return await base.SendAsync(req, cancellationToken);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var x = new HttpMessageHandlerBuilder()
                .AddHandler(new CustomHandler())
                .Build();
        }
    }
}
