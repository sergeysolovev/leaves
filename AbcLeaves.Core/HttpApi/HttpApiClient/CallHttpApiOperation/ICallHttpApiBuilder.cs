using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AbcLeaves.Core
{
    public interface ICallHttpApiBuilder : ICallHttpApiRequestBuilder
    {
        ICallHttpApiOperation Build();
    }

    public interface ICallHttpApiRequestBuilder
    {
        ICallHttpApiRequestBuilder UseRequestContent(Func<HttpContent> getRequestContent);
        ICallHttpApiRequestBuilder AddRequestHeaders(Action<HttpRequestHeaders> addRequestHeaders);
        ICallHttpApiRequestBuilder AddRequestUrlParameter(string name, string value);
        ICallHttpApiBuilder CompleteRequest(HttpMethod method, string url);
    }
}