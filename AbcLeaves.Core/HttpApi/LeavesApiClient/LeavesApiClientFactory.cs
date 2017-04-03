using System;

namespace AbcLeaves.Core
{
    public class LeavesApiClientFactory : IHttpApiClientFactory
    {
        private readonly IHttpApiClientServiceFactory clientServiceFactory;

        public LeavesApiClientFactory(IHttpApiClientServiceFactory clientServiceFactory)
        {
            if (clientServiceFactory == null)
            {
                throw new ArgumentNullException(nameof(clientServiceFactory));
            }

            this.clientServiceFactory = clientServiceFactory;
        }

        public IHttpApiClient Create(IHttpApiOptions apiOptions)
        {
            return LeavesApiClient.Create(clientServiceFactory, apiOptions);
        }
    }
}
