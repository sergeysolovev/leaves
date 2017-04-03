using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;

namespace AbcLeaves.Core
{
    public class CallHttpApiBuilder : ICallHttpApiBuilder
    {
        private readonly ICallHttpApiFactory callApiFactory;
        private readonly IHttpApiOptions apiOptions;
        private readonly Uri baseUri;
        private readonly string apiName;

        private HttpMethod method;
        private string apiEndpoint;
        private Action<HttpRequestHeaders> addRequestHeaders;
        private Func<HttpContent> getRequestContent;
        private Dictionary<string, string> queryString = new Dictionary<string, string>();

        public static CallHttpApiBuilder Create(
            IHttpApiOptions apiOptions,
            ICallHttpApiFactory callApiFactory)
        {
            return new CallHttpApiBuilder(apiOptions, callApiFactory);
        }

        private CallHttpApiBuilder(
            IHttpApiOptions apiOptions,
            ICallHttpApiFactory callApiFactory)
        {
            if (apiOptions == null)
            {
                throw new ArgumentNullException(nameof(apiOptions));
            }
            if (String.IsNullOrEmpty(apiOptions.Name))
            {
                throw new ArgumentNullException(nameof(apiOptions.Name));
            }
            if (String.IsNullOrEmpty(apiOptions.BaseUrl))
            {
                throw new ArgumentNullException(nameof(apiOptions.BaseUrl));
            }
            if (callApiFactory == null)
            {
                throw new ArgumentException(nameof(callApiFactory));
            }

            this.apiOptions = apiOptions;
            this.callApiFactory = callApiFactory;
            this.apiName = apiOptions.Name;
            this.baseUri = EstablishBaseUri(apiOptions.BaseUrl);
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
            var operation = callApiFactory.Create(apiOptions);
            var operationParams = (ICallHttpApiOperationParams)operation;
            operationParams.ApiEndpoint = apiEndpoint;
            operationParams.ApiName = apiName;
            operationParams.Method = method;
            operationParams.AddRequestHeaders = addRequestHeaders;
            operationParams.GetRequestContent = getRequestContent;
            ResetMutableState();
            return operation;
        }

        private void ResetMutableState()
        {
            method = null;
            apiEndpoint = null;
            addRequestHeaders = null;
            getRequestContent = null;
            queryString.Clear();
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
