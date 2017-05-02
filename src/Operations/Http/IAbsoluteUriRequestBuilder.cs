using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Operations.Http
{
    public interface IAbsoluteUriRequestBuilder : IOperationBuilder<HttpRequestMessage>
    {
        IAbsoluteUriRequestBuilder ConfigureHeaders(Action<HttpRequestHeaders> configure);
        IAbsoluteUriRequestBuilder WithContent(HttpContent content);
        IAbsoluteUriRequestBuilder WithBearerToken(string token);
        IAbsoluteUriRequestBuilder AddParameter(string name, string value);
        //IOperation<HttpRequestMessage> Build();
    }
}