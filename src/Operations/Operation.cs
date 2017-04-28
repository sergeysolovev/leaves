using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Operations.Linq
{
    public static class Operation
    {
        public static IOperation<T> Get<T>(Func<Task<IResult<T>>> valueFactory)
            => new Operation<T>(valueFactory);

        public static IOperation<T> Get<T>(Func<IResult<T>> valueFactory)
            => new Operation<T>(() => Wrap(valueFactory()));

        public static IOperation<T> Get<T>(Func<T> valueFactory)
            => Get<T>(() => Result.Just(valueFactory()));

        public static IOperation<T> Get<T>(T value)
            => Get<T>(() => Result.Just(value));

        public static IOperation<T> None<T>()
            => Get<T>(() => Result.None<T>());

        public static IOperation<T> Where<T>(
            this IOperation<T> source,
            Func<T, bool> predicate)
            => Get(() => source.ExecuteAsync().Bind(x =>
                x.HasValue && predicate(x.Value) ?
                    x.Wrap() :
                    Result.None<T, T>(x).Wrap()));

        public static IOperation<U> Select<T, U>(
            this IOperation<T> source,
            Func<T, U> selector)
            => Get(() => source.ExecuteAsync().Bind(x =>
                x.HasValue ?
                    Result.Just<U>(selector(x.Value)).Wrap() :
                    Result.None<T, U>(x).Wrap()));

        public static IOperation<U> Bind<T, U>(
            this IOperation<T> source,
            Func<T, IOperation<U>> selector)
            => Get(() => source.ExecuteAsync().Bind(x =>
                x.HasValue ?
                    selector(x.Value).ExecuteAsync() :
                    Result.None<T, U>(x).Wrap()));

        public static IOperation<U> SelectMany<T, V, U>(
            this IOperation<T> source,
            Func<T, IOperation<V>> selector,
            Func<T, V, U> resultSelector)
            => Bind(source, Compose(selector, resultSelector));

        private static Func<T, IOperation<U>> Compose<T, V, U>(
            Func<T, IOperation<V>> selector,
            Func<T, V, U> resultSelector)
            => x => Get(() => selector(x).ExecuteAsync().Bind(y =>
                y.HasValue ?
                    Result.Just<U>(resultSelector(x, y.Value)).Wrap() :
                    Result.None<V, U>(y).Wrap()));

        private static Task<T> Wrap<T>(this T value)
            => Task.FromResult(value);

        private static async Task<U> Bind<T, U>(
            this Task<T> source,
            Func<T, Task<U>> selector)
            => await selector(await source);
    }
}