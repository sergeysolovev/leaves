using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Operations.Extensions.Http
{
    public interface IRelativeUriRequestBuilder : IBuilder<HttpRequestMessage, IRelativeUriRequestBuilder>
    {
        IRelativeUriRequestBuilder WithRelativeRef(string url, HttpMethod method);
        IRelativeUriRequestBuilder ConfigureHeaders(Action<HttpRequestHeaders> configure);
        IRelativeUriRequestBuilder WithContent(HttpContent content);
        IRelativeUriRequestBuilder WithBearerToken(string token);
        IRelativeUriRequestBuilder AddParameter(string name, string value);
    }
}