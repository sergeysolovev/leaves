using System;
using System.Threading.Tasks;

namespace Operations
{
    public sealed class Builder<T> : Builder<T, Builder<T>>
    {
        internal Builder(IOperation<T> source) : base(source) { }
        internal Builder(T source) : base(source) { }
    }

    public static class Builder
    {
        public static void Test()
        {
            // benefits:
            // make Operation<T> out of T and use it in queries => can hide Operation.Returns
            // use With clauses to extend the value of T

            var getClient = Builder
                .Return(new System.Net.Http.HttpClient())
                .Build();

            // builder examples:
            // HttpClientBuilder
            // HttpRequestBuilder
            // HttpMessageHandlerBuilder
            // none of them needs async

            ValueTask<object> x;
        }

        public static IBuilder<T, Builder<T>> Return<T>(
            IOperation<T> source)
            => new Builder<T>(source);

        public static IBuilder<T, Builder<T>> Return<T>(
            T source)
            => new Builder<T>(source);

        public static TBuilder With<TResult, TBuilder>(
            this IBuilder<TResult, TBuilder> source,
            IOperationService<TResult> service)
            => source.Return(source.Build().Bind(service));

        public static TBuilder With<TResult, TBuilder>(
            this IBuilder<TResult, TBuilder> source,
            Func<TResult, IOperation<TResult>> closure)
            => source.Return(source.Build().Bind(closure));

        public static TBuilder With<TResult, TBuilder>(
            this IBuilder<TResult, TBuilder> source,
            Func<TResult, Task<IContext<TResult>>> closure)
            => source.Return(source.Build().Bind(closure));

        public static TBuilder With<TResult, TBuilder>(
            this IBuilder<TResult, TBuilder> source,
            Func<TResult, IContext<TResult>> closure)
            => source.Return(source.Build().Bind(closure));
    }
}