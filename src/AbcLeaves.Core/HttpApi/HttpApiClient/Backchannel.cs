using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace AbcLeaves.Core
{
    internal class JsonContent : StringContent
    {
        public JsonContent(string content)
            : this(content, Encoding.UTF8)
        {
        }

        public JsonContent(string content, Encoding encoding)
            : base(content, encoding, "application/json")
        {
        }
    }

    public interface IHttpRequestBuilder
    {
        IHttpRequestBuilder WithBearerToken(string token);
        IHttpRequestBuilder WithContent(HttpContent content);
        IHttpRequestBuilder WithJsonContent(string json);
        IHttpRequestBuilder WithJsonContent(string json, Encoding encoding);
        IHttpRequestBuilder AcceptJson();
        IHttpRequestBuilder ConfigureHeaders(Action<HttpRequestHeaders> configure);
        IHttpRequestBuilder AddParameter(string name, string value);
        HttpRequestMessage Build(string url, HttpMethod method);
    }

    internal class HttpRequestBuilder : IHttpRequestBuilder
    {
        private readonly string baseUrl;
        private readonly Dictionary<string, string> queryString;
        private readonly HttpRequestMessage request;

        private static string[] httpSchemas = new [] { "http", "https" };

        private static bool TryCreateAbsoluteUri(string url, out Uri uri)
            =>  Uri.TryCreate(url, UriKind.Absolute, out uri) &&
                Enumerable.Contains(httpSchemas, uri.Scheme);

        public HttpRequestBuilder(string baseUrl = null)
        {
            this.baseUrl = baseUrl;
            this.queryString = new Dictionary<string, string>();
            this.request = new HttpRequestMessage();
        }

        public IHttpRequestBuilder AddParameter(string name, string value)
        {
            queryString.Add(
                Throw.IfNullOrEmpty(name, nameof(name)),
                Throw.IfNull(value, nameof(value))
            );
            return this;
        }

        public IHttpRequestBuilder ConfigureHeaders(Action<HttpRequestHeaders> configure)
        {
            configure?.Invoke(request.Headers);
            return this;
        }

        public IHttpRequestBuilder WithBearerToken(string token)
        {
            if (request.Headers.Authorization != null)
            {
                throw new InvalidOperationException(
                    "The request auth header has been already set");
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return this;
        }

        public IHttpRequestBuilder WithContent(HttpContent content)
        {
            request.Content = Throw.IfNull(content, nameof(content));
            return this;
        }

        public IHttpRequestBuilder WithJsonContent(string json)
        {
            return WithContent(new JsonContent(json));
        }

        public IHttpRequestBuilder WithJsonContent(string json, Encoding encoding)
        {
            return WithContent(new JsonContent(json, encoding));
        }

        public IHttpRequestBuilder AcceptJson()
        {
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
            return this;
        }

        //.ConfigureHeaders(headers => headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")))

        public HttpRequestMessage Build(string url, HttpMethod method)
        {
            Uri endpointUri = null;
            if (baseUrl != null)
            {
                if (!TryCreateAbsoluteUri(baseUrl, out Uri baseUri))
                {
                    throw new ArgumentException(
                        $"{baseUrl} is not a valid http base URI. See RFC 3986 5.1",
                        nameof(baseUrl)
                    );
                }
                if (!Uri.TryCreate(url, UriKind.Relative, out Uri relativeRefUri))
                {
                    throw new ArgumentException(
                        $"{url} is not a valid relative reference. See RFC 3986 4.2"
                    );
                }
                if (!Uri.TryCreate(baseUri, relativeRefUri, out endpointUri))
                {
                    throw new ArgumentException(
                        $"Failed to resolve api endpoint URI from base URI {baseUri} " +
                        $"and relative referece {relativeRefUri}. See RFC 3986 5.2"
                    );
                }
            }
            else
            {
                if (!TryCreateAbsoluteUri(url, out endpointUri))
                {
                    throw new ArgumentException(
                        $"{url} is not a valid http absolute URI. See RFC 3986 4.3"
                    );
                }
            }

            request.Method = method;
            request.RequestUri = new Uri(
                QueryHelpers.AddQueryString(endpointUri.ToString(), queryString)
            );

            return request;
        }
    }

    public class SendMessageResult : IDisposable
    {
        private Exception error;
        private HttpResponseMessage response;
        private bool disposed;

        public HttpResponseMessage Response => disposed ?
            throw new ObjectDisposedException(nameof(response)) :
            response;

        public Exception Error => error;

        public bool Succeeded => (Error == null);

        private SendMessageResult(HttpResponseMessage response)
            => this.response = Throw.IfNull(response, nameof(response));

        private SendMessageResult(Exception error)
            => this.error = Throw.IfNull(error, nameof(error));

        public static SendMessageResult Succeed(HttpResponseMessage response)
            => new SendMessageResult(response);

        public static SendMessageResult Fail(Exception error)
            => new SendMessageResult(error);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Response != null)
                {
                    Response.Dispose();
                    this.disposed = true;
                }
            }
        }
    }

    public interface IBackchannel
    {
        Task<SendMessageResult> SendAsync(
            string url,
            HttpMethod method,
            Action<IHttpRequestBuilder> configure = null
        );
    }

    public interface IBackchannelFactory
    {
        IBackchannel Create(string baseUrl = null);
    }

    internal class BackchannelFactory : IBackchannelFactory
    {
        private readonly HttpMessageHandler httpMessageHandler;

        public BackchannelFactory(HttpMessageHandler httpMessageHandler)
            => this.httpMessageHandler = Throw.IfNull(
                httpMessageHandler,
                nameof(httpMessageHandler)
            );

        public IBackchannel Create(string baseUrl = null)
            => new Backchannel(httpMessageHandler, baseUrl);
    }

    internal class Backchannel : IBackchannel
    {
        private readonly string baseUrl;
        private readonly HttpClient client;

        public Backchannel(HttpMessageHandler httpMessageHandler, string baseUrl = null)
        {
            this.baseUrl = baseUrl;
            this.client = new HttpClient(
                Throw.IfNull(httpMessageHandler, nameof(httpMessageHandler))
            );
        }

        public async Task<SendMessageResult> SendAsync(string url, HttpMethod method,
            Action<IHttpRequestBuilder> configure = null)
        {
            var request = buildRequest();

            try
            {
                var response = await client.SendAsync(request);
                return SendMessageResult.Succeed(response);
            }
            catch (HttpRequestException error)
            {
                return SendMessageResult.Fail(error);
            }

            HttpRequestMessage buildRequest()
            {
                var builder = new HttpRequestBuilder(baseUrl);
                configure?.Invoke(builder);
                return builder.Build(url, method);
            }
        }
    }

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
