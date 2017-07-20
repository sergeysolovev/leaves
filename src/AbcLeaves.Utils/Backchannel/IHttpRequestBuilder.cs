using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AbcLeaves.Utils
{
    public interface IHttpRequestBuilder
    {
        IHttpRequestBuilder WithBearerToken(string token);
        IHttpRequestBuilder WithContent(HttpContent content);
        IHttpRequestBuilder WithJsonContent(string json);
        IHttpRequestBuilder WithJsonContent(string json, Encoding encoding);
        IHttpRequestBuilder AcceptJson();
        IHttpRequestBuilder ConfigureHeaders(Action<HttpRequestHeaders> configure);
        IHttpRequestBuilder AddParameter(string name, string value);
        HttpRequestMessage Build(string url, HttpMethod method);
    }
}
