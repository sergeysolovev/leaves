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
            => new Operation<T>(() => Task.FromResult(valueFactory()));

        public static IOperation<T> Get<T>(Func<Task<T>> valueFactory)
            => new Operation<T>(() => Wrap(valueFactory()));

        public static IOperation<T> Get<T>(Func<T> valueFactory)
            => Get<T>(() => Result.Just(valueFactory()));

        public static IOperation<T> Get<T>(T value)
            => Get<T>(() => Result.Just(value));

        public static IOperation<T> None<T>()
            => Get<T>(() => Result.None<T>());

        public static IOperation<T> Where<T>(
            this IOperation<T> self,
            Func<T, bool> predicate)
            => new Operation<T>(() =>
                self.ExecuteAsync().ContinueWith(x =>
                    HasValue(x) && predicate(GetValue(x)) ?
                        Result.Just<T>(GetValue(x)) :
                        Result.None<T, T>(x.Result)));

        public static IOperation<U> Select<T, U>(
            this IOperation<T> self,
            Func<T, U> selector)
            => new Operation<U>(() =>
                self.ExecuteAsync().ContinueWith(x =>
                    HasValue(x) ?
                        Result.Just<U>(selector(GetValue(x))) :
                        Result.None<T, U>(x.Result)));

        public static IOperation<U> SelectMany<T, V, U>(
            this IOperation<T> self,
            Func<T, IOperation<V>> selector,
            Func<T, V, U> resultSelector)
            => Select(self, Compose(selector, resultSelector));

        private static Func<T, U> Compose<T, V, U>(
            Func<T, IOperation<V>> selector,
            Func<T, V, U> resultSelector)
            => x => resultSelector(x, GetValue(selector(x)));

        private static Task<IResult<T>> Wrap<T>(Task<T> source)
            => source.ContinueWith(x => Result.Just<T>(source.Result));

        private static bool HasValue<T>(Task<IResult<T>> source)
            => source.Result.HasValue;

        private static T GetValue<T>(Task<IResult<T>> source)
            => source.Result.Value;

        private static T GetValue<T>(IOperation<T> source)
            => source.ExecuteAsync().Result.Value;
    }
}