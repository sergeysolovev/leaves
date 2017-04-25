using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AbcLeaves.Core
{
    public interface ICallHttpApiRequestBuilder
    {
        ICallHttpApiRequestBuilder AddBearerToken(string bearerToken);
        ICallHttpApiRequestBuilder UseRequestContent(Func<HttpContent> getRequestContent);
        ICallHttpApiRequestBuilder AddRequestHeaders(Action<HttpRequestHeaders> addRequestHeaders);
        ICallHttpApiRequestBuilder AddRequestUrlParameter(string name, string value);
        ICallHttpApiBuilder CompleteRequest(HttpMethod method, string url);
    }
}