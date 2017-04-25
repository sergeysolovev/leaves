using System;
using System.Linq;
using System.Collections.Generic;

namespace AbcLeaves.Core
{
    internal class IdentifyHttpRequestComposite :
        CompositeOperation<MapHttpRequestResult, MapHttpRequestContext>,
        IIdentifyHttpRequest
    {
        public static IdentifyHttpRequestComposite Create(
            IEnumerable<IIdentifyHttpRequest> operations)
        {
            return new IdentifyHttpRequestComposite(operations);
        }

        private IdentifyHttpRequestComposite(IEnumerable<IIdentifyHttpRequest> operations)
        {
            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            AddOperations(operations);
        }

        private void AddOperations(IEnumerable<IIdentifyHttpRequest> operations)
        {
            operations.Where(op => op != this).ToList().ForEach(op => Add(op));
        }
    }
}
