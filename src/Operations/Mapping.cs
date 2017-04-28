using System;
using System.Collections.Generic;
using System.Linq;
using Operations.Linq;

namespace Operations
{
    internal class Mapping<T> : IMapping<T>
    {
        private readonly Func<IOperation<T>, IOperation<T>> map;

        public Mapping(Func<IOperation<T>, IOperation<T>> mapping)
            => map = Throw.IfNull(mapping, nameof(mapping));

        public IOperation<T> Map(IOperation<T> arg)
            => map(arg);
    }

    public static class Mapping
    {
        public static IMapping<T> Id<T>()
            => Get<T>((IOperation<T> x) => x);

        public static IMapping<T> Get<T>(Func<IOperation<T>, IOperation<T>> mapping)
            => new Mapping<T>(mapping);

        public static IMapping<T> Get<T>(Func<T, T> mapping)
            => Get<T>(x => Operation.Get<T>(() => mapping(x.ExecuteAsync().Result.Value)));

        public static IMapping<T> Compose<T>(this IEnumerable<IMapping<T>> mappings)
            => Enumerable.Aggregate(mappings, Id<T>(), Compose);

        public static IMapping<T> Compose<T>(this IMapping<T> left, IMapping<T> right)
            => Mapping.Get<T>(x => left.Map(right.Map(x)));
    }
}