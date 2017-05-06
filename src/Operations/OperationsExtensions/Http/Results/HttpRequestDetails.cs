using System;
using System.Collections.Generic;

namespace Operations.Extensions.Http
{
    public sealed class HttpRequestDetails
    {
        public string Endpoint { get; }
        public string Method { get; }
        public string Body { get; }
        public string Headers { get; }

        public HttpRequestDetails(
            string endpoint,
            string method,
            string body,
            string headers)
        {
            Endpoint = endpoint;
            Method = method;
            Body = body ?? String.Empty;
            Headers = headers;
        }

        public IDictionary<string, object> ToDictionary()
            => new Dictionary<string, object> {
                ["endpoint"] = Endpoint,
                ["headers"] = Headers,
                ["method"] = Method,
                ["body"] = Body
            };
    }
}