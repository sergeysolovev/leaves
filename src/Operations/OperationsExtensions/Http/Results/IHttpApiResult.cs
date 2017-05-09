namespace Operations.Extensions.Http
{
    public interface IHttpApiResult
    {
        HttpRequestDetails RequestDetails { get; }
        HttpResponseDetails ResponseDetails { get; }
    }

    internal class HttpApiResult : IHttpApiResult
    {
        public HttpRequestDetails RequestDetails { get; }
        public HttpResponseDetails ResponseDetails { get; }

        internal HttpApiResult(
            HttpRequestDetails requestDetails,
            HttpResponseDetails responseDetails)
        {
            RequestDetails = Throw.IfNull(requestDetails, nameof(requestDetails));
            ResponseDetails = Throw.IfNull(responseDetails, nameof(responseDetails));
        }
    }
}