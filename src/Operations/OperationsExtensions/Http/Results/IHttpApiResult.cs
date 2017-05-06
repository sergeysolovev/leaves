namespace Operations.Extensions.Http
{
    public interface IHttpApiResult
    {
        HttpRequestDetails RequestDetails { get; }
        HttpResponseDetails ResponseDetails { get; }
    }
}