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
            => Get<T>(() => Result.Succeed(valueFactory()));

        public static IOperation<T> Get<T>(T value)
            => Get<T>(() => Result.Succeed(value));

        public static IOperation<T> None<T>()
            => Get<T>(() => Result.None<T>());

        public static IOperation<T> Where<T>(
            this IOperation<T> source,
            Func<T, bool> predicate)
            => Get(() => source.ExecuteAsync().Bind(x =>
                x.Succeeded && predicate(x.Value) ?
                    x.Wrap() :
                    Result.FailFrom(x).Wrap()));

        public static IOperation<U> Select<T, U>(
            this IOperation<T> source,
            Func<T, U> selector)
            => Get(() => source.ExecuteAsync().Bind(x =>
                x.Succeeded ?
                    Result.Succeed(selector(x.Value)).Wrap() :
                    Result.FailFrom<T, U>(x).Wrap()));

        public static IOperation<U> Bind<T, U>(
            this IOperation<T> source,
            Func<T, IOperation<U>> selector)
            => Get(() => source.ExecuteAsync().Bind(x =>
                x.Succeeded ?
                    selector(x.Value).ExecuteAsync() :
                    Result.FailFrom<T, U>(x).Wrap()));

        public static IOperation<U> SelectMany<T, V, U>(
            this IOperation<T> source,
            Func<T, IOperation<V>> selector,
            Func<T, V, U> resultSelector)
            => Bind(source, Compose(selector, resultSelector));

        private static Func<T, IOperation<U>> Compose<T, V, U>(
            Func<T, IOperation<V>> selector,
            Func<T, V, U> resultSelector)
            => x => Get(() => selector(x).ExecuteAsync().Bind(y =>
                y.Succeeded ?
                    Result.Succeed(resultSelector(x, y.Value)).Wrap() :
                    Result.FailFrom<V, U>(y).Wrap()));

        private static Task<T> Wrap<T>(this T value)
            => Task.FromResult(value);

        private static async Task<U> Bind<T, U>(
            this Task<T> source,
            Func<T, Task<U>> selector)
            => await selector(await source);
    }
}