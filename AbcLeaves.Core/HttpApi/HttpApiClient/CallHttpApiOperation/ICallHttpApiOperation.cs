using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public interface ICallHttpApiOperation : ICallHttpApiOperationParams
    {
        Task<CallHttpApiResult> ExecuteAsync();
    }

    public interface ICallHttpApiOperationParams
    {
        HttpMethod Method { get; set; }
        string ApiEndpoint { get; set; }
        string ApiName { get; set; }
        Action<HttpRequestHeaders> AddRequestHeaders { get; set; }
        Func<HttpContent> GetRequestContent { get; set; }
    }
}
