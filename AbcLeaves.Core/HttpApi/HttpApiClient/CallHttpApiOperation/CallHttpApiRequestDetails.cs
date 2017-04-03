using System;
using System.Collections.Generic;

namespace AbcLeaves.Core
{
    public sealed class CallHttpApiRequestDetails
    {
        public string Endpoint { get; private set; }
        public string Method { get; private set; }
        public string Body { get; private set; }
        public string Headers { get; private set; }

        public CallHttpApiRequestDetails(
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

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object> {
                ["endpoint"] = Endpoint,
                ["headers"] = Headers,
                ["method"] = Method,
                ["body"] = Body
            };
        }
    }
}
