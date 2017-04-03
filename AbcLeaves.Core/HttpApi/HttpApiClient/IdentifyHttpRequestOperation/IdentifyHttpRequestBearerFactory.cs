using System;

namespace AbcLeaves.Core
{
    public class IdentifyHttpRequestBearerFactory : IdentifyHttpRequestFactory
    {
        private readonly IBearerTokenProvider bearerTokenProvider;

        public IdentifyHttpRequestBearerFactory(IBearerTokenProvider bearerTokenProvider)
        {
            if (bearerTokenProvider == null)
            {
                throw new InvalidOperationException(nameof(bearerTokenProvider));
            }
            this.bearerTokenProvider = bearerTokenProvider;
        }

        public override HttpApiAuthType AuthType => HttpApiAuthType.Bearer;

        public override IIdentifyHttpRequestOperation Create()
        {
            return IdentifyHttpRequestBearer.Create(bearerTokenProvider);
        }
    }
}
