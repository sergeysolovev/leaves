using System.Collections.Generic;
using System.Net.Http;

namespace AbcLeaves.Core
{
    public class MapHttpRequestResult : OperationResultBase<MapHttpRequestContext>
    {
        public HttpRequestMessage Request => Context.Request;

        public MapHttpRequestResult() : base() { }

        protected MapHttpRequestResult(MapHttpRequestContext context, bool succeeded)
            : base(context, succeeded)
        {
        }

        protected MapHttpRequestResult(HttpRequestMessage request, bool succeeded)
            : this(new MapHttpRequestContext(request), succeeded)
        {
        }

        protected MapHttpRequestResult(
            HttpRequestMessage request,
            string error,
            Dictionary<string, object> details)
            : base(new MapHttpRequestContext(request), error, details)
        {
        }

        public static MapHttpRequestResult Success(HttpRequestMessage request)
            => new MapHttpRequestResult(request, succeeded: true);

        public static MapHttpRequestResult Success(MapHttpRequestContext context)
            => new MapHttpRequestResult(context, succeeded: true);

        public static MapHttpRequestResult Fail(
            HttpRequestMessage request,
            string error,
            Dictionary<string, object> details = null)
        {
            return new MapHttpRequestResult(request, error, details);
        }
    }
}
