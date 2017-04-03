using System;

namespace AbcLeaves.Core
{
    public class CallHttpApiFactory : ICallHttpApiFactory
    {
        private readonly IIdentifyHttpApiRequestFactory identifyApiRequestFactory;

        public CallHttpApiFactory(
            IIdentifyHttpApiRequestFactory identifyApiRequestFactory)
        {
            if (identifyApiRequestFactory == null)
            {
                throw new ArgumentNullException(nameof(identifyApiRequestFactory));
            }

            this.identifyApiRequestFactory = identifyApiRequestFactory;
        }

        public ICallHttpApiOperation Create(IHttpApiOptions apiOptions)
        {
            var requestIdentifier = identifyApiRequestFactory.Create(apiOptions);
            var callApiOperation = CallHttpApi.Create(requestIdentifier);
            if (apiOptions.Backchannel != null && callApiOperation.Backchannel == null)
            {
                callApiOperation.Backchannel = apiOptions.Backchannel;
            }
            return callApiOperation;
        }
    }
}
