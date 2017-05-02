using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Operations.Linq;

namespace Operations
{
    public static class OperationService
    {
        public static IOperationService<T, U> Return<T, U>(
            Func<T, IOperation<U>> closure)
            => new OperationService<T, U>(closure);

        public static IOperationService<T, U> Return<T, U>(
            Func<T, Task<IContext<U>>> closure)
            => Return<T, U>(x => Operation.Return(() => closure(x)));

        public static IOperationService<T, U> Return<T, U>(
            Func<T, IContext<U>> closure)
            => Return<T, U>(x => Operation.Return(() => closure(x)));

        public static IOperationService<T, T> Id<T>()
            => Return<T, T>(x => Operation.Return(x));

        public static Func<T, IOperation<U>> ToClosure<T, U>(
            this IOperationService<T, U> source)
            => x => source.Return(x);

        public static IOperationService<T, U> AddInner<T, V, U>(
            this IOperationService<V, U> source,
            IOperationService<T, V> inner)
            => Return<T, U>(x => inner.Return(x).Bind(source));

        public static IOperationService<T, U> AddInner<T, V, U>(
            this IOperationService<V, U> source,
            Func<T, IOperation<V>> inner)
            => Return<T, U>(x => inner(x).Bind(ToClosure(source)));

        public static IOperationService<T, U> AddInner<T, V, U>(
            this IOperationService<V, U> source,
            Func<T, Task<IContext<V>>> inner)
            => AddInner(source, Return(inner));

        public static IOperationService<T, U> AddInner<T, U, V>(
            this IOperationService<V, U> source,
            Func<T, IContext<V>> inner)
            => AddInner(source, Return(inner));

        public static IOperationService<T, U> AddOuter<T, V, U>(
            this IOperationService<T, V> source,
            IOperationService<V, U> outer)
            => Return<T, U>(x => source.Return(x).Bind(outer));

        public static IOperationService<T, U> AddOuter<T, V, U>(
            this IOperationService<T, V> source,
            Func<V, IOperation<U>> outer)
            => Return<T, U>(x => source.Return(x).Bind(outer));

        public static IOperationService<T, U> AddOuter<T, V, U>(
            this IOperationService<T, V> source,
            Func<V, Task<IContext<U>>> inner)
            => AddOuter(source, Return(inner));

        public static IOperationService<T, U> AddOuter<T, V, U>(
            this IOperationService<T, V> source,
            Func<V, IContext<U>> inner)
            => AddOuter(source, Return(inner));
    }
}