using System.Net.Http;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public abstract class IdentifyHttpRequest : IIdentifyHttpRequestOperation
    {
        private static SkipIdentifyingHttpRequest skipIdentifyingRequest;

        public static IdentifyHttpRequest None
        {
            get
            {
                if (skipIdentifyingRequest == null)
                {
                    skipIdentifyingRequest = new SkipIdentifyingHttpRequest();
                }
                return skipIdentifyingRequest;
            }
        }

        public abstract Task<IdentifyHttpRequestResult> ExecuteAsync(HttpRequestMessage request);

        internal class SkipIdentifyingHttpRequest : IdentifyHttpRequest
        {
            public override async Task<IdentifyHttpRequestResult> ExecuteAsync(
                HttpRequestMessage request)
            {
                return await Task.FromResult(IdentifyHttpRequestResult.Success(request));
            }
        }
    }
}
