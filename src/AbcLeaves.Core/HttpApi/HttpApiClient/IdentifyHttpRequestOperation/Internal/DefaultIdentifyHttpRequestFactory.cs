using System;
using System.Collections.Generic;

namespace AbcLeaves.Core
{
    internal class DefaultIdentifyHttpRequestFactory
    {
        private readonly IEnumerable<IIdentifyHttpRequest> operations;

        public DefaultIdentifyHttpRequestFactory(IEnumerable<IIdentifyHttpRequest> operations)
        {
            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            this.operations = operations;
        }

        public IIdentifyHttpRequest Create()
        {
            return IdentifyHttpRequestComposite.Create(operations);
        }
    }
}
