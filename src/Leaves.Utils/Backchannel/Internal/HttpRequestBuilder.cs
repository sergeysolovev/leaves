using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace Leaves.Utils
{
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
}
