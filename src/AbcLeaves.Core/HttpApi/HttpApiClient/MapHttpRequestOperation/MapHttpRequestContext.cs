using System;
using System.Net.Http;

namespace AbcLeaves.Core
{
    public class MapHttpRequestContext : IOperationContext
    {
        public MapHttpRequestContext()
        {
        }

        public MapHttpRequestContext(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            Request = request;
        }

        public HttpRequestMessage Request { get; set; }
    }
}
