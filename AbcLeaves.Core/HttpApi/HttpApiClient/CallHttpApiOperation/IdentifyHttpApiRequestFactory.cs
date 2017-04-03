using System;
using System.Collections.Generic;

namespace AbcLeaves.Core
{
    public class IdentifyHttpApiRequestFactory : IIdentifyHttpApiRequestFactory
    {
        private readonly Dictionary<HttpApiAuthType, IIdentifyHttpRequestFactory> factories;

        public static IdentifyHttpApiRequestFactory Create()
        {
            return new IdentifyHttpApiRequestFactory();
        }

        private IdentifyHttpApiRequestFactory()
        {
            factories = new Dictionary<HttpApiAuthType, IIdentifyHttpRequestFactory>();
            RegisterIdentifyHttpRequestFactory(IdentifyHttpRequestFactory.SkipIdentifyingFactory);
        }

        public void RegisterIdentifyHttpRequestFactory(IIdentifyHttpRequestFactory factory)
        {
            var authType = factory.AuthType;
            if (IsRegisteredAuthType(authType))
            {
                var error = $"A factory for auth type {authType} has already been registered";
                throw new InvalidOperationException(error);
            }
            factories.Add(authType, factory);
        }

        public bool IsRegisteredAuthType(HttpApiAuthType authType)
        {
            return factories.ContainsKey(authType);
        }

        public IIdentifyHttpRequestOperation Create(IHttpApiOptions apiOptions)
        {
            if (apiOptions == null)
            {
                throw new ArgumentNullException(nameof(apiOptions));
            }
            IIdentifyHttpRequestFactory factory;
            HttpApiAuthType authType = apiOptions.AuthType;
            if (!factories.TryGetValue(authType, out factory))
            {
                var error = $"A factory for auth type {authType} is not registered";
                throw new InvalidOperationException(error);
            }
            return factory.Create();
        }
    }
}
