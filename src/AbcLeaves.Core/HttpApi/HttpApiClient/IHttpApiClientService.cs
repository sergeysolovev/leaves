using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public interface IHttpApiClientService
    {
        Task<CallHttpApiResult> CallAsync(HttpMethod method, string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null);

        Task<CallHttpApiResult> GetAsync(string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null);

        Task<CallHttpApiResult> PostAsync(string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null);

        Task<CallHttpApiResult> DeleteAsync(string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null);

        Task<CallHttpApiResult> PutAsync(string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null);

        Task<CallHttpApiResult> PatchAsync(string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null);
    }
}
