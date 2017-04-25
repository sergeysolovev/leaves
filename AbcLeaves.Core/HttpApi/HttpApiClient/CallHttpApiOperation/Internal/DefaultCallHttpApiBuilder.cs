using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;

namespace AbcLeaves.Core
{
    internal class DefaultCallHttpApiBuilder : ICallHttpApiBuilder
    {
        private readonly ICallHttpApiOptions options;
        private readonly IHttpBackchannel backchannel;
        private readonly Uri baseUri;
        private readonly string apiName;

        private HttpMethod method;
        private string apiEndpoint;
        private Action<HttpRequestHeaders> addRequestHeaders;
        private Func<HttpContent> getRequestContent;
        private Dictionary<string, string> queryString = new Dictionary<string, string>();
        private IdentifyHttpRequestComposite identifyRequestComposite;

        public static DefaultCallHttpApiBuilder Create(
            ICallHttpApiOptions apiOptions,
            IHttpBackchannel backchannel,
            IIdentifyHttpRequest identifyRequest)
        {
            return new DefaultCallHttpApiBuilder(apiOptions, backchannel, identifyRequest);
        }

        private DefaultCallHttpApiBuilder(
            ICallHttpApiOptions options,
            IHttpBackchannel backchannel,
            IIdentifyHttpRequest identifyRequest)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (String.IsNullOrEmpty(options.Name))
            {
                throw new ArgumentNullException(nameof(options.Name));
            }
            if (String.IsNullOrEmpty(options.BaseUrl))
            {
                throw new ArgumentNullException(nameof(options.BaseUrl));
            }
            if (identifyRequest == null)
            {
                throw new ArgumentNullException(nameof(identifyRequest));
            }
            if (backchannel == null)
            {
                throw new ArgumentNullException(nameof(backchannel));
            }

            this.options = options;
            this.apiName = options.Name;
            this.baseUri = EstablishBaseUri(options.BaseUrl);
            this.backchannel = options.Backchannel ?? backchannel;
            this.identifyRequestComposite = IdentifyHttpRequestComposite.Create(
                identifyRequest.Yield());
        }

        ICallHttpApiRequestBuilder ICallHttpApiRequestBuilder.AddBearerToken(string bearerToken)
        {
            if (String.IsNullOrEmpty(bearerToken))
            {
                throw new ArgumentNullException(nameof(bearerToken));
            }

            identifyRequestComposite.Add(IdentifyHttpRequestBearer.Create(bearerToken));
            return this;
        }

        ICallHttpApiRequestBuilder ICallHttpApiRequestBuilder.UseRequestContent(
            Func<HttpContent> getRequestContent)
        {
            if (getRequestContent == null)
            {
                throw new ArgumentNullException(nameof(getRequestContent));
            }

            this.getRequestContent = getRequestContent;
            return this;
        }

        ICallHttpApiRequestBuilder ICallHttpApiRequestBuilder.AddRequestHeaders(
            Action<HttpRequestHeaders> addRequestHeaders)
        {
            if (addRequestHeaders == null)
            {
                throw new ArgumentNullException(nameof(addRequestHeaders));
            }

            this.addRequestHeaders += addRequestHeaders;
            return this;
        }

        ICallHttpApiRequestBuilder ICallHttpApiRequestBuilder.AddRequestUrlParameter(
            string name, string value)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            queryString.Add(name, value);
            return this;
        }

        ICallHttpApiBuilder ICallHttpApiRequestBuilder.CompleteRequest(
            HttpMethod method, string relativeRef)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (relativeRef == null)
            {
                throw new ArgumentNullException(nameof(relativeRef));
            }

            var relativeUri = GetRelativeReference(relativeRef);
            this.apiEndpoint = ResolveApiEndpoint(baseUri, relativeUri);
            this.method = method;
            return this;
        }

        ICallHttpApiOperation ICallHttpApiBuilder.Build()
        {
            return new DefaultCallHttpApi
            {
                Backchannel = backchannel,
                IdentifyRequest = identifyRequestComposite,
                ApiEndpoint = apiEndpoint,
                ApiName = apiName,
                Method = method,
                AddRequestHeaders = addRequestHeaders,
                GetRequestContent = getRequestContent
            };
        }

        private Uri EstablishBaseUri(string baseUrl)
        {
            Uri baseUri;
            bool isValid =
                Uri.TryCreate(baseUrl, UriKind.Absolute, out baseUri) &&
                new [] { "http", "https" }.Contains(baseUri.Scheme);
            if (!isValid)
            {
                var error = $"{baseUrl} is not a valid http api base URI. See RFC 3986 5.1";
                throw new ArgumentException(error, nameof(baseUrl));
            }
            return baseUri;
        }

        private Uri GetRelativeReference(string relativeRef)
        {
            Uri relativeReference;
            if (!Uri.TryCreate(relativeRef, UriKind.Relative, out relativeReference))
            {
                var error = $"{relativeRef} is not a valid relative reference. See RFC 3986 4.2";
                throw new ArgumentException(error, nameof(relativeRef));
            }
            return relativeReference;
        }

        private string ResolveApiEndpoint(Uri baseUri, Uri relativeRef)
        {
            Uri apiEndpoint;
            if (!Uri.TryCreate(baseUri, relativeRef, out apiEndpoint))
            {
                var error =
                    $"Failed to resolve api endpoint URI from base URI {baseUri} " +
                    $"and relative referece {relativeRef}. See RFC 3986 5.2";
                throw new ArgumentException(error);
            }
            return QueryHelpers.AddQueryString(apiEndpoint.ToString(), queryString);
        }
    }
}
