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
            => source.Return(service.AddInner(x => source.BuildWith(x)));

        public static TBuilder With<TResult, TBuilder>(
            this IBuilder<TResult, TBuilder> source,
            Func<TResult, IOperation<TResult>> closure)
            where TBuilder : IBuilder<TResult, TBuilder>
            => With(source, OperationService.Return<TResult>(closure));

        public static TBuilder With<TResult, TBuilder>(
            this IBuilder<TResult, TBuilder> source,
            Func<TResult, Task<IContext<TResult>>> closure)
            where TBuilder : IBuilder<TResult, TBuilder>
            => With(source, OperationService.Return<TResult>(closure));

        public static TBuilder With<TResult, TBuilder>(
            this IBuilder<TResult, TBuilder> source,
            Func<TResult, IContext<TResult>> closure)
            where TBuilder : IBuilder<TResult, TBuilder>
            => With(source, OperationService.Return<TResult>(closure));
    }
}