using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Operations
{
    public static class Context
    {
        public static IContext<T> Default<T>()
            => new Context<T>(null);

        public static IContext<T> Succeed<T>(T value, Dictionary<string, object> props = null)
            => new Context<T>(value, props);

        public static IContext<T> Fail<T>(T value, Exception error, Dictionary<string, object> props = null)
            => new Context<T>(value, error, props);

        public static IContext<T> Fail<T>(T value, string error, Dictionary<string, object> props = null)
            => new Context<T>(value, new InvalidOperationException(error), props);

        public static IContext<T> FailFrom<T>(IContext<T> source)
            => new Context<T>(source.Result, source.Error, source.Properties);

        public static IContext<U> FailFrom<T, U>(IContext<T> source)
            => source is IContext<U> result ?
                FailFrom(result) :
                new Context<U>(source.Error, source.Properties);
    }
}