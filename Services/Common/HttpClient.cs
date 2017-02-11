using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ABC.Leaves.Api.Services
{
    public class HttpClient : IHttpClient
    {
        private readonly System.Net.Http.HttpClient httpClient;

        public HttpClient()
        {
            httpClient = new System.Net.Http.HttpClient();
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return await httpClient.PostAsync(requestUri, content);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, string accessToken)
        {
            using(var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content })
            {
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                return await httpClient.SendAsync(request);
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await httpClient.GetAsync(requestUri);
        }

        public void Dispose()
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
            }
        }
    }
}