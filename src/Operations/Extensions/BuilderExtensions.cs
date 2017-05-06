using System;
using System.Threading.Tasks;

namespace Operations
{
    public static class BuilderExtensions
    {
        public static TReturn With<TResult, TReturn>(
            this IBuilder<TResult, TReturn> source,
            IOperationService<TResult> service)
            => source.Return(source.Build().Bind(service));

        public static TReturn With<TResult, TReturn>(
            this IBuilder<TResult, TReturn> source,
            Func<TResult, Task<IContext<TResult>>> closure)
            => source.Return(source.Build().Bind(closure));

        public static TReturn With<TResult, TReturn>(
            this IBuilder<TResult, TReturn> source,
            Func<TResult, IContext<TResult>> closure)
            => source.Return(source.Build().Bind(closure));

        public static TReturn With<TResult, TReturn>(
            this IBuilder<TResult, TReturn> source,
            Func<TResult, TResult> selector)
            => source.Return(source.Build().Select(selector));

        public static TReturn When<TResult, TReturn>(
            this IBuilder<TResult, TReturn> source,
            Func<TResult, bool> predicate)
            => source.Return(source.Build().Where(predicate));
    }
}