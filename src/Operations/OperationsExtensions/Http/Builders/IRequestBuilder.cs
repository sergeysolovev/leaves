using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Operations.Extensions.Http
{
    public interface IRequestBuilder
    {
        IRelativeUriRequestBuilder UseBaseUri(string baseUrl);
        IAbsoluteUriRequestBuilder UseAbsoluteUri(string url, HttpMethod method);
    }
}