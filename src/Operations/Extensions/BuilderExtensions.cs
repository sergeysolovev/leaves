using System;
using System.Threading.Tasks;

namespace Operations
{
    public static class BuilderExtensions
    {
        public static TBuilder With<TResult, TBuilder>(
            this IBuilder<TResult, TBuilder> source,
            IOperationService<TResult> service)
            where TBuilder : IBuilder<TResult, TBuilder>
            => source.Return(source.Build().Bind(service));

        public static TBuilder With<TResult, TBuilder>(
            this IBuilder<TResult, TBuilder> source,
            Func<TResult, Task<IContext<TResult>>> closure)
            where TBuilder : IBuilder<TResult, TBuilder>
            => source.Return(source.Build().Bind(closure));

        public static TBuilder With<TResult, TBuilder>(
            this IBuilder<TResult, TBuilder> source,
            Func<TResult, IContext<TResult>> closure)
            where TBuilder : IBuilder<TResult, TBuilder>
            => source.Return(source.Build().Bind(closure));

        public static TBuilder With<TResult, TBuilder>(
            this IBuilder<TResult, TBuilder> source,
            Func<TResult, TResult> selector)
            where TBuilder : IBuilder<TResult, TBuilder>
            => source.Return(source.Build().Select(selector));

        public static TBuilder When<TResult, TBuilder>(
            this IBuilder<TResult, TBuilder> source,
            Func<TResult, bool> predicate)
            where TBuilder : IBuilder<TResult, TBuilder>
            => source.Return(source.Build().Where(predicate));
    }
}