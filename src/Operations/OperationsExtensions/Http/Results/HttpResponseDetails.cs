using System;
using System.Collections.Generic;
using System.Net;

namespace Operations.Extensions.Http
{
    public sealed class HttpResponseDetails
    {
        public string Body { get; }
        public string Headers { get; }
        public HttpStatusCode StatusCode { get; }
        public bool IsSuccessStatusCode => ((int)StatusCode >= 200) && ((int)StatusCode <= 299);
        public string StatusMessage => StatusCode.ToString();

        public HttpResponseDetails(string body, string headers, HttpStatusCode statusCode)
        {
            Body = body ?? String.Empty;
            Headers = headers;
            StatusCode = statusCode;
        }

        public IDictionary<string, object> ToDictionary()
            => new Dictionary<string, object> {
                ["statusCode"] = StatusCode,
                ["statusMessage"] = StatusMessage,
                ["headers"] = Headers,
                ["body"] = Body
            };
    }
}