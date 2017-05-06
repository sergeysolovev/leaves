using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Operations.Extensions.Http
{
    public interface IAbsoluteUriRequestBuilder : IBuilder<HttpRequestMessage, IAbsoluteUriRequestBuilder>
    {
        IAbsoluteUriRequestBuilder ConfigureHeaders(Action<HttpRequestHeaders> configure);
        IAbsoluteUriRequestBuilder WithContent(HttpContent content);
        IAbsoluteUriRequestBuilder WithBearerToken(string token);
        IAbsoluteUriRequestBuilder AddParameter(string name, string value);
    }
}