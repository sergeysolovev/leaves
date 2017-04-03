using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public class HttpApiClientService : IHttpApiClientService
    {
        private readonly ICallHttpApiBuilderFactory callApiBuilderFactory;
        private readonly IHttpApiOptions apiOptions;
        private readonly HttpMethod httpPatch = new HttpMethod("PATCH");

        public static HttpApiClientService Create(
            ICallHttpApiBuilderFactory callApiBuilderFactory,
            IHttpApiOptions apiOptions)
        {
            return new HttpApiClientService(callApiBuilderFactory, apiOptions);
        }

        private HttpApiClientService(
            ICallHttpApiBuilderFactory callApiBuilderFactory,
            IHttpApiOptions apiOptions)
        {
            if (callApiBuilderFactory == null)
            {
                throw new ArgumentNullException(nameof(callApiBuilderFactory));
            }
            if (apiOptions == null)
            {
                throw new ArgumentNullException(nameof(apiOptions));
            }

            this.callApiBuilderFactory = callApiBuilderFactory;
            this.apiOptions = apiOptions;
        }

        protected virtual ICallHttpApiBuilder CreateCallHttpApiBuilder()
        {
            return callApiBuilderFactory.Create(apiOptions);
        }

        public async Task<CallHttpApiResult> CallAsync(HttpMethod method, string url,
            Action<ICallHttpApiRequestBuilder> builderAction = null)
        {
            var builder = CreateCallHttpApiBuilder();
            builderAction = builderAction ?? (x => {});
            builderAction.Invoke(builder);
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
            return await CallAsync(httpPatch, url, builderAction);
        }
    }
}
