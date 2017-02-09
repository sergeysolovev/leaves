using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ABC.Leaves.Api.Services
{
    public interface IHttpClient : IDisposable
    {
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
    }
}
