using System;

namespace AbcLeaves.Core
{
    internal class DefaultCallHttpApiBuilderFactory : ICallHttpApiBuilderFactory
    {
        private readonly IIdentifyHttpRequestFactory identifyRequestFactory;
        private readonly IHttpBackchannel backchannel;

        public DefaultCallHttpApiBuilderFactory(
            IHttpBackchannel backchannel,
            IIdentifyHttpRequestFactory identifyRequestFactory)
        {
            if (backchannel == null)
            {
                throw new ArgumentNullException(nameof(backchannel));
            }
            if (identifyRequestFactory == null)
            {
                throw new ArgumentNullException(nameof(identifyRequestFactory));
            }

            this.backchannel = backchannel;
            this.identifyRequestFactory = identifyRequestFactory;
        }

        public ICallHttpApiBuilder Create(ICallHttpApiOptions apiOptions)
        {
            var identifyRequest = identifyRequestFactory.Create();
            return DefaultCallHttpApiBuilder.Create(apiOptions, backchannel, identifyRequest);
        }
    }
}
