using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;

namespace Operations.Http
{
    internal partial class RequestBuilder : IRequestBuilder, IAbsoluteUriRequestBuilder, IRelativeUriRequestBuilder
    {
        internal class BuilderContext
        {
            internal Uri BaseUri;
            internal Uri AbsoluteUri;
            internal Uri RelativeRef;
            internal HttpMethod Method;
            internal Dictionary<string, string> QueryString;
            internal HttpRequestMessage Request;

            internal bool IsBearerSet
                => (Request.Headers.Authorization != null);

            internal void SetBearer(string token)
                => Request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

            internal HttpRequestMessage BuildRequest()
            {
                Request.RequestUri = new Uri(QueryHelpers.AddQueryString(
                    AbsoluteUri.ToString(),
                    QueryString));
                return Request;
            }

            internal BuilderContext()
            {
                QueryString = new Dictionary<string, string>();
                Request = new HttpRequestMessage();
            }
        }
    }
}