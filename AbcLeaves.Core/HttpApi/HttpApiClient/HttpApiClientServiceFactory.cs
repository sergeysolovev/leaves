using System;

namespace AbcLeaves.Core
{
    public class HttpApiClientServiceFactory : IHttpApiClientServiceFactory
    {
        private readonly ICallHttpApiBuilderFactory callApiBuilderFactory;

        public HttpApiClientServiceFactory(ICallHttpApiBuilderFactory callApiBuilderFactory)
        {
            if (callApiBuilderFactory == null)
            {
                throw new ArgumentNullException(nameof(callApiBuilderFactory));
            }

            this.callApiBuilderFactory = callApiBuilderFactory;
        }

        public IHttpApiClientService Create(IHttpApiOptions apiOptions)
        {
            return HttpApiClientService.Create(callApiBuilderFactory, apiOptions);
        }
    }
}
