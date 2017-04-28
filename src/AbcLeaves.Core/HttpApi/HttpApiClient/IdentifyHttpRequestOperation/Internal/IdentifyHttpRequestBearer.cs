using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    internal class IdentifyHttpRequestBearer : MapHttpRequest, IIdentifyHttpRequest
    {
        private readonly IBearerTokenProvider bearerTokenProvider;

        internal sealed class SingleBearerTokenProvider : IBearerTokenProvider
        {
            private readonly string bearerToken;

            private SingleBearerTokenProvider(string bearerToken)
            {
                if (String.IsNullOrEmpty(bearerToken))
                {
                    throw new ArgumentNullException();
                }

                this.bearerToken = bearerToken;
            }

            internal static SingleBearerTokenProvider Create(string bearerToken)
            {
                return new SingleBearerTokenProvider(bearerToken);
            }

            public Task<AuthTokenResult> GetBearerToken()
            {
                return Task.FromResult(AuthTokenResult.Success(bearerToken));
            }
        }

        public static IdentifyHttpRequestBearer Create(string bearerToken)
        {
            var provider = SingleBearerTokenProvider.Create(bearerToken);
            return new IdentifyHttpRequestBearer(provider);
        }

        public IdentifyHttpRequestBearer(IBearerTokenProvider bearerTokenProvider)
        {
            if (bearerTokenProvider == null)
            {
                throw new ArgumentNullException(nameof(bearerTokenProvider));
            }

            this.bearerTokenProvider = bearerTokenProvider;
        }

        public override async Task<MapHttpRequestResult> ExecuteAsync(
            MapHttpRequestContext context)
        {
            return await this
                .BeginWith(() => bearerTokenProvider.GetBearerToken())
                .EndWith(bearer => AddBearer(context.Request, bearer.Token))
                .Return();
        }

        private MapHttpRequestResult AddBearer(
            HttpRequestMessage request, string bearerToken)
        {
            if (request.Headers.Authorization != null)
            {
                return MapHttpRequestResult.Fail(request,
                    "The request auth header has been already set");
            }
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer", bearerToken);
            return MapHttpRequestResult.Success(request);
        }
    }
}
