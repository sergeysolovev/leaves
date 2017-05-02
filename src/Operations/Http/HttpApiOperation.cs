using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Operations.Linq;

namespace Operations.Http
{
    public interface IHttpApiService
    {
        IRequestBuilder GetRequestBuilder();
        IOperation<HttpRequestMessage> GetRequest(string url, HttpMethod method);
        IOperation<IHttpApiResult> GetResponse(HttpRequestMessage request);
    }

    public interface IHttpApiResult
    {
        HttpRequestDetails RequestDetails { get; }
        HttpResponseDetails ResponseDetails { get; }
    }

    public class HttpApiService : IHttpApiService
    {
        private readonly IHttpBackchannel backchannel;

        internal HttpApiService(IHttpBackchannel backchannel)
            => this.backchannel = Throw.IfNull(backchannel, nameof(backchannel));

        public IRequestBuilder GetRequestBuilder()
            => new RequestBuilder();

        public IOperation<HttpRequestMessage> GetRequest(string url, HttpMethod method)
            => GetRequestBuilder().UseAbsoluteUri(url, method).Build();

        public IOperation<IHttpApiResult> GetResponse(HttpRequestMessage request)
            => throw new NotImplementedException();
    }

    public static class HttpApiServiceExtensions
    {
        // public static IOperation<HttpRequestMessage> BuildGet(this IHttpApiService service, string url)
        //     => service.GetRequest(url, HttpMethod.Get);

        // public static IOperation<HttpRequestMessage> BuildPost(this IHttpApiService service, string url)
        //     => service.GetRequest(url, HttpMethod.Post);

        // public static IOperation<HttpRequestMessage> BuildDelete(this IHttpApiService service, string url)
        //     => service.GetRequest(url, HttpMethod.Delete);

        // public static IOperation<HttpRequestMessage> BuildPut(this IHttpApiService service, string url)
        //     => service.GetRequest(url, HttpMethod.Put);

        // public static IOperation<HttpRequestMessage> BuildPatch(this IHttpApiService service, string url)
        //     => service.GetRequest(url, new HttpMethod(HttpMethods.Patch));

        // public static IOperation<HttpRequestMessage> AddBearerToken(
        //     this IOperation<HttpRequestMessage> source,
        //     string token)
        //     => source.Bind(request => {
        //         if (request.Headers.Authorization != null)
        //         {
        //             return Result.Fail(request, "The request auth header has been already set");
        //         }
        //         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //         return Result.Succeed(request);
        //     });
    }

    public static class CallHttpApiServiceTests
    {
        public static void Test()
        {
            IHttpApiService service = null;
            var getRequest = service.GetRequest("url", HttpMethod.Get);

            var rb = service.GetRequestBuilder()
                .UseAbsoluteUri("", null)
                .WithBearerToken("")
                .Build();

            var requestBuilder = service
                .GetRequestBuilder()
                .UseBaseUri("http://localhost:8080/api/v1")
                .WithBearerToken("jpoajv9j-30j4v39jv093j43");

            var r = from request in requestBuilder
                        .WithRelativeRef("leaves", HttpMethod.Post)
                        .AddParameter("name", "value")
                        .Build()
                    from response in service.GetResponse(request)
                    select response;
        }
    }

    public interface IHttpBackchannel
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}