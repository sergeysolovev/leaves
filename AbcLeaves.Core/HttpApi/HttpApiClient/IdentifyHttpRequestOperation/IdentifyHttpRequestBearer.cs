using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public class IdentifyHttpRequestBearer : IdentifyHttpRequest
    {
        private readonly IBearerTokenProvider bearerTokenProvider;

        public static IdentifyHttpRequestBearer Create(
            IBearerTokenProvider bearerTokenProvider)
        {
            return new IdentifyHttpRequestBearer(bearerTokenProvider);
        }

        private IdentifyHttpRequestBearer(IBearerTokenProvider bearerTokenProvider)
        {
            if (bearerTokenProvider == null)
            {
                throw new ArgumentNullException(nameof(bearerTokenProvider));
            }

            this.bearerTokenProvider = bearerTokenProvider;
        }

        public override async Task<IdentifyHttpRequestResult> ExecuteAsync(
            HttpRequestMessage request)
        {
            return await Operation<IdentifyHttpRequestResult>
                .BeginWith(() => bearerTokenProvider.GetBearerToken())
                .EndWith(bearer => AddBearer(request, bearer.Token))
                .Return();
        }

        private IdentifyHttpRequestResult AddBearer(
            HttpRequestMessage request, string bearerToken)
        {
            if (request.Headers.Authorization != null)
            {
                return IdentifyHttpRequestResult.Fail(
                    "The request auth header has been already set");
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            return IdentifyHttpRequestResult.Success(request);
        }
    }
}
