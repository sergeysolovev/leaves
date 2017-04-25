using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AbcLeaves.Core
{
    public class HttpApiClientService : IHttpApiClientService
    {
        private readonly ICallHttpApiOptions options;
        private readonly ICallHttpApiBuilderFactory builderFactory;

        public static HttpApiClientService Create(
            ICallHttpApiOptions options,
            ICallHttpApiBuilderFactory builderFactory)
        {
            return new HttpApiClientService(options, builderFactory);
        }

        public HttpApiClientService(
            ICallHttpApiOptions apiOptions,
            ICallHttpApiBuilderFactory builderFactory)
        {
            if (apiOptions == null)
            {
                throw new ArgumentNullException(nameof(apiOptions));
            }
            if (builderFactory == null)
            {
                throw new ArgumentNullException(nameof(builderFactory));
            }

            this.options = apiOptions;
            this.builderFactory = builderFactory;
        }

        public async Task<CallHttpApiResult> CallAsync(HttpMethod method, string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null)
        {
            var builder = builderFactory.Create(options);
            builderAction?.Invoke(builder);
            var operation = builder
                .CompleteRequest(method, url)
                .Build();
            return await operation.ExecuteAsync();
        }

        public async Task<CallHttpApiResult> GetAsync(string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null)
        {
            return await CallAsync(HttpMethod.Get, url, builderAction);
        }

        public async Task<CallHttpApiResult> PostAsync(string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null)
        {
            return await CallAsync(HttpMethod.Post, url, builderAction);
        }

        public async Task<CallHttpApiResult> DeleteAsync(string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null)
        {
            return await CallAsync(HttpMethod.Delete, url, builderAction);
        }

        public async Task<CallHttpApiResult> PutAsync(string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null)
        {
            return await CallAsync(HttpMethod.Put, url, builderAction);
        }

        public async Task<CallHttpApiResult> PatchAsync(string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null)
        {
            return await CallAsync(new HttpMethod(HttpMethods.Patch), url, builderAction);
        }
    }
}
