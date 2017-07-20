using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AbcLeaves.Utils
{
    public static class BackchannelExtensions
    {
        public static async Task<SendMessageResult> GetAsync(
            this IBackchannel client,
            string url,
            Action<IHttpRequestBuilder> configure = null)
        {
            return await client.SendAsync(url, HttpMethod.Get, configure);
        }

        public static async Task<SendMessageResult> PostAsync(
            this IBackchannel backchannel,
            string url,
            Action<IHttpRequestBuilder> configure = null)
        {
            return await backchannel.SendAsync(url, HttpMethod.Post, configure);
        }

        public static async Task<SendMessageResult> DeleteAsync(
            this IBackchannel backchannel,
            string url,
            Action<IHttpRequestBuilder> configure = null)
        {
            return await backchannel.SendAsync(url, HttpMethod.Delete, configure);
        }

        public static async Task<SendMessageResult> PutAsync(
            this IBackchannel backchannel,
            string url,
            Action<IHttpRequestBuilder> configure = null)
        {
            return await backchannel.SendAsync(url, HttpMethod.Put, configure);
        }

        public static async Task<SendMessageResult> PatchAsync(
            this IBackchannel backchannel,
            string url,
            Action<IHttpRequestBuilder> configure = null)
        {
            return await backchannel.SendAsync(url, new HttpMethod("PATCH"), configure);
        }
    }
}
