using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Operations.Linq
{
    public static class Operation
    {
        public static IOperation<T> Return<T>(Func<Task<IContext<T>>> valueFactory)
            => new Operation<T>(valueFactory);

        public static IOperation<T> Return<T>(Func<IContext<T>> valueFactory)
            => new Operation<T>(() => Wrap(valueFactory()));

        public static IOperation<T> Return<T>(Func<T> valueFactory)
            => Return<T>(() => Context.Succeed(valueFactory()));

        public static IOperation<T> Return<T>(T value)
            => Return<T>(() => Context.Succeed(value));

        public static IOperation<T> None<T>()
            => Return<T>(() => Context.None<T>());

        public static IOperation<T> Where<T>(
            this IOperation<T> source,
            Func<T, bool> predicate)
            => Return(() => source.ExecuteAsync().Bind(x =>
                x.Succeeded && predicate(x.Result) ?
                    x.Wrap() :
                    Context.FailFrom(x).Wrap()));

        public static IOperation<U> Select<T, U>(
            this IOperation<T> source,
            Func<T, U> selector)
            => Return(() => source.ExecuteAsync().Bind(x =>
                x.Succeeded ?
                    Context.Succeed(selector(x.Result)).Wrap() :
                    Context.FailFrom<T, U>(x).Wrap()));

        public static IOperation<U> SelectMany<T, V, U>(
            this IOperation<T> source,
            Func<T, IOperation<V>> closure,
            Func<T, V, U> selector)
            => Bind(source, Compose(closure, selector));

        public static IOperation<U> Bind<T, U>(
            this IOperation<T> source,
            Func<T, IOperation<U>> closure)
            => Return(() => source.ExecuteAsync().Bind(x =>
                x.Succeeded ?
                    closure(x.Result).ExecuteAsync() :
                    Context.FailFrom<T, U>(x).Wrap()));

        public static IOperation<U> Bind<T, U>(
            this IOperation<T> source,
            IOperationService<T, U> service)
            => Bind(source, service.ToClosure());

        public static IOperation<U> Bind<T, U>(
            this IOperation<T> source,
            Func<T, Task<IContext<U>>> closure)
            => Bind(source, OperationService.Return(closure));

        public static IOperation<U> Bind<T, U>(
            this IOperation<T> source,
            Func<T, IContext<U>> closure)
            => Bind(source, OperationService.Return(closure));

        private static Func<T, IOperation<U>> Compose<T, V, U>(
            Func<T, IOperation<V>> closure,
            Func<T, V, U> selector)
            => x => Return(() => closure(x).ExecuteAsync().Bind(y =>
                y.Succeeded ?
                    Context.Succeed(selector(x, y.Result)).Wrap() :
                    Context.FailFrom<V, U>(y).Wrap()));

        private static Task<T> Wrap<T>(this T value)
            => Task.FromResult(value);

        private static async Task<U> Bind<T, U>(
            this Task<T> source,
            Func<T, Task<U>> selector)
            => await selector(await source);
    }
}