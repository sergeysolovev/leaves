using System;
using System.Threading.Tasks;

namespace Operations
{
    public static class PreBuilder
    {
        public static IOperationBuilder<TResult> InjectNew<TResult, TBuilder>(
            this IPreBuilder<TResult, TBuilder> source)
            where TResult : new()
            => source.Inject(new TResult());

        public static TBuilder With<TResult, TBuilder>(
            this IPreBuilder<TResult, TBuilder> source,
            IOperationService<TResult> service)
            => source.Return(service.AddInner(x => source.Inject(x).Build()));

        public static TBuilder With<TResult, TBuilder>(
            this IPreBuilder<TResult, TBuilder> source,
            Func<TResult, IOperation<TResult>> closure)
            => With(source, OperationService.Return<TResult>(closure));

        public static TBuilder With<TResult, TBuilder>(
            this IPreBuilder<TResult, TBuilder> source,
            Func<TResult, Task<IContext<TResult>>> closure)
            => With(source, OperationService.Return<TResult>(closure));

        public static TBuilder With<TResult, TBuilder>(
            this IPreBuilder<TResult, TBuilder> source,
            Func<TResult, IContext<TResult>> closure)
            => With(source, OperationService.Return<TResult>(closure));
    }
}