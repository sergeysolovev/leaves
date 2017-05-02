using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Operations.Linq;

namespace Operations.Http
{
    internal partial class RequestBuilder :
        IRequestBuilder,
        IAbsoluteUriRequestBuilder,
        IRelativeUriRequestBuilder
    {
        private readonly OperationBuilder<BuilderContext> builder;

        internal RequestBuilder()
            => builder = new OperationBuilder<BuilderContext>(new BuilderContext());

        IAbsoluteUriRequestBuilder IRequestBuilder.UseAbsoluteUri(string url, HttpMethod method)
            => Inject(
                context => TryCreateAbsoluteUri(url, out context.AbsoluteUri) ?
                    Context.Succeed(context).With(x => x.Method = method) :
                    Context.Fail(context, new ArgumentException(
                        $"{url} is not a valid http absolute URI. See RFC 3986 4.3",
                        nameof(url))));

        IRelativeUriRequestBuilder IRequestBuilder.UseBaseUri(string baseUrl)
            => Inject(
                context => TryCreateAbsoluteUri(baseUrl, out context.BaseUri) ?
                    Context.Succeed(context) :
                    Context.Fail(context, new ArgumentException(
                        $"{baseUrl} is not a valid http base URI. See RFC 3986 5.1",
                        nameof(baseUrl))));

        IRelativeUriRequestBuilder IRelativeUriRequestBuilder.WithRelativeRef(
            string url, HttpMethod method)
            => Inject(context =>
                Uri.TryCreate(url, UriKind.Relative, out context.RelativeRef) ?
                    Uri.TryCreate(context.BaseUri, context.RelativeRef, out context.AbsoluteUri) ?
                        Context.Succeed(context) :
                        Context.Fail(context, new ArgumentException(
                            $"Failed to resolve api endpoint URI from base URI {context.BaseUri} " +
                            $"and relative referece {url}. See RFC 3986 5.2")) :
                    Context.Fail(context, new ArgumentException(
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

        IOperation<HttpRequestMessage> IRelativeUriRequestBuilder.Build()
            => Build();

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

        IOperation<HttpRequestMessage> IOperationBuilder<HttpRequestMessage>.Build()
            => Build();

        void IOperationBuilder<HttpRequestMessage>.Inject(
            IOperationService<HttpRequestMessage, HttpRequestMessage> service)
        {
            throw new NotImplementedException();
        }

        private RequestBuilder ConfigureHeaders(
            Action<HttpRequestHeaders> configure)
            => Inject(
                context => Context
                    .Succeed(context)
                    .With(x => configure(x.Request.Headers)));

        private RequestBuilder AddParameter(string name, string value)
            => Inject(
                context => Context
                    .Succeed(context)
                    .With(x => x.QueryString.Add(
                        Throw.IfNullOrEmpty(name, nameof(name)),
                        Throw.IfNull(value, nameof(value)))));

        private RequestBuilder WithContent(HttpContent content)
            => Inject(
                context => Context
                    .Succeed(context)
                    .With(x => x.Request.Content =
                        Throw.IfNull(content, nameof(content))));

        private RequestBuilder WithBearerToken(string token)
            => Inject(
                context => context.IsBearerSet ?
                    Context.Fail(context,
                        "The request auth header has been already set") :
                    Context.Succeed(context).With(x => x.SetBearer(token)));

        private IOperation<HttpRequestMessage> Build()
            => builder.Build().Bind(
                context => Context.Succeed(context.BuildRequest()));

        private RequestBuilder Inject(Func<BuilderContext, IContext<BuilderContext>> closure)
        {
            builder.Inject(closure);
            return this;
        }

        private static string[] httpSchemas = new [] { "http", "https" };

        private static bool TryCreateAbsoluteUri(string url, out Uri uri)
            =>  Uri.TryCreate(url, UriKind.Absolute, out uri) &&
                Enumerable.Contains(httpSchemas, uri.Scheme);
    }
}