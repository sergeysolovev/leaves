using System.Net.Http;

namespace Operations.Extensions.Http
{
    public interface IGetHttpApiResponse : IOperationService<HttpRequestMessage, IHttpApiResult>
    {
    }
}