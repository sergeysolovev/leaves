using System;
using System.Collections.Generic;
using System.Net;

namespace AbcLeaves.Core
{
    public sealed class CallHttpApiResponseDetails
    {
        public string Body { get; private set; }
        public string Headers { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public bool IsSuccessStatusCode => ((int)StatusCode >= 200) && ((int)StatusCode <= 299);
        public string StatusMessage => StatusCode.ToString();

        public CallHttpApiResponseDetails(string body, string headers, HttpStatusCode statusCode)
        {
            Body = body ?? String.Empty;
            Headers = headers;
            StatusCode = statusCode;
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object> {
                ["statusCode"] = StatusCode,
                ["statusMessage"] = StatusMessage,
                ["headers"] = Headers,
                ["body"] = Body
            };
        }
    }
}
