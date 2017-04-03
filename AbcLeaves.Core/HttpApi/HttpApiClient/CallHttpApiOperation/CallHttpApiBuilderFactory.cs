using System;

namespace AbcLeaves.Core
{
    public class CallHttpApiBuilderFactory : ICallHttpApiBuilderFactory
    {
        private readonly ICallHttpApiFactory callApiFactory;

        public CallHttpApiBuilderFactory(
            ICallHttpApiFactory callApiFactory)
        {
            if (callApiFactory == null)
            {
                throw new ArgumentNullException(nameof(callApiFactory));
            }

            this.callApiFactory = callApiFactory;
        }

        public ICallHttpApiBuilder Create(IHttpApiOptions apiOptions)
        {
            return CallHttpApiBuilder.Create(apiOptions, callApiFactory);
        }
    }
}
