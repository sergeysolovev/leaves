using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Operations
{
    public class CompositeMapping<T> : IMapping<T>
    {
        private readonly IMapping<T> composite;

        public CompositeMapping(IEnumerable<IMapping<T>> mappings)
            => composite = Throw.IfNull(mappings, nameof(mappings)).Compose();

        public IOperation<T> Map(IOperation<T> arg)
            => composite.Map(arg);
    }
}