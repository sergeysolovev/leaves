using System.Net.Http;

namespace Operations.Http
{
    public interface IRequestBuilder
    {
        IRelativeUriRequestBuilder UseBaseUri(string baseUrl);
        IAbsoluteUriRequestBuilder UseAbsoluteUri(string url, HttpMethod method);
    }
}