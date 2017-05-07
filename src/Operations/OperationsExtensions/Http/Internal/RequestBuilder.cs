using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace Operations.Extensions.Http
{
    internal class RequestBuilder :
        Builder<HttpRequestMessage, RequestBuilder>,
        IRequestBuilder,
        IAbsoluteUriRequestBuilder,
        IRelativeUriRequestBuilder
    {
        IAbsoluteUriRequestBuilder IBuilder<HttpRequestMessage, IAbsoluteUriRequestBuilder>.Return(
            IOperationService<HttpRequestMessage> source)
            => Return(source);

        IRelativeUriRequestBuilder IBuilder<HttpRequestMessage, IRelativeUriRequestBuilder>.Return(
            IOperationService<HttpRequestMessage> source)
            => Return(source);

        IAbsoluteUriRequestBuilder IRequestBuilder.UseAbsoluteUri(
            string url, HttpMethod method)
            => With(request => TryCreateAbsoluteUri(url, out Uri requestUri) ?
                Context.Succeed(request)
                    .With(x => x.Method = method)
                    .With(x => x.RequestUri = requestUri) :
                Context.Fail(request, new ArgumentException(
                    $"{url} is not a valid http absolute URI. See RFC 3986 4.3",
                    nameof(url))));

        IRelativeUriRequestBuilder IRequestBuilder.UseBaseUri(
            string baseUrl)
            => With(request => TryCreateAbsoluteUri(baseUrl, out Uri baseUri) ?
                Context.Succeed(request)
                    .With(x => x.RequestUri = baseUri) :
                Context.Fail(request, new ArgumentException(
                    $"{baseUrl} is not a valid http base URI. See RFC 3986 5.1",
                    nameof(baseUrl))));

        IRelativeUriRequestBuilder IRelativeUriRequestBuilder.WithRelativeRef(
            string url, HttpMethod method)
            => With(request => Uri.TryCreate(url, UriKind.Relative, out Uri relativeRef) ?
                Uri.TryCreate(request.RequestUri, relativeRef, out Uri requestUri) ?
                    Context.Succeed(request).With(x => x.RequestUri = requestUri) :
                    Context.Fail(request, new ArgumentException(
                        $"Failed to resolve api endpoint URI from base URI {request.RequestUri} " +
                        $"and relative referece {url}. See RFC 3986 5.2")) :
                Context.Fail(request, new ArgumentException(
                    $"{url} is not a valid relative reference. See RFC 3986 4.2",
                    nameof(url))));

        IRelativeUriRequestBuilder IRelativeUriRequestBuilder.ConfigureHeaders(
            Action<HttpRequestHeaders> configure)
            => ConfigureHeaders(configure);

        IRelativeUriRequestBuilder IRelativeUriRequestBuilder.AddParameter(
            string name, string value)
            => AddParameter(name, value);

        IRelativeUriRequestBuilder IRelativeUriRequestBuilder.WithContent(
            HttpContent content)
            => WithContent(content);

        IRelativeUriRequestBuilder IRelativeUriRequestBuilder.WithBearerToken(
            string token)
            => WithBearerToken(token);

        IAbsoluteUriRequestBuilder IAbsoluteUriRequestBuilder.ConfigureHeaders(
            Action<HttpRequestHeaders> configure)
            => ConfigureHeaders(configure);

        IAbsoluteUriRequestBuilder IAbsoluteUriRequestBuilder.AddParameter(
            string name, string value)
            => AddParameter(name, value);

        IAbsoluteUriRequestBuilder IAbsoluteUriRequestBuilder.WithContent(
            HttpContent content)
            => WithContent(content);

        IAbsoluteUriRequestBuilder IAbsoluteUriRequestBuilder.WithBearerToken(
            string token)
            => WithBearerToken(token);

        private RequestBuilder ConfigureHeaders(
            Action<HttpRequestHeaders> configure)
            => With(request => Context
                .Succeed(request)
                .With(x => configure(x.Headers)));

        private RequestBuilder AddParameter(string name, string value)
            => With(request => Context
                .Succeed(request)
                .With(x => AddRequestParameter(x, name, value)));

        private RequestBuilder WithContent(HttpContent content)
            => With(request => Context
                .Succeed(request)
                .With(x => x.Content = Throw.IfNull(content, nameof(content))));

        private RequestBuilder WithBearerToken(string token)
            => With(request => !IsBearerSet(request) ?
                Context.Succeed(request).With(x => SetBearer(x, token)) :
                Context.Fail(request, "The request auth header has been already set"));

        private static string[] httpSchemas = new [] { "http", "https" };

        private static bool TryCreateAbsoluteUri(string url, out Uri uri)
            =>  Uri.TryCreate(url, UriKind.Absolute, out uri) &&
                Enumerable.Contains(httpSchemas, uri.Scheme);

        private static void AddRequestParameter(HttpRequestMessage request, string name, string value)
        {
            request.RequestUri = new Uri(QueryHelpers.AddQueryString(
                uri: request.RequestUri.ToString(),
                name: Throw.IfNullOrEmpty(name, nameof(name)),
                value: Throw.IfNull(value, nameof(value))));
        }

        internal static bool IsBearerSet(HttpRequestMessage request)
            => request.Headers.Authorization != null;

        internal void SetBearer(HttpRequestMessage request, string token)
            => request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}