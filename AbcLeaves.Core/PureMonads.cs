using System;

namespace LazyMonadExperiments
{
    // Lazy<T>: Functor, Monad, Pure regardless T
    // => LazyAsync<T> is Functor, Monad, Pure regardless T
    // Func<T>: Functor, Monad, Pure regardless T

    static class LazyExtensions
    {
        public static Lazy<TResult> SelectMany<TSource, TNext, TResult>(
            this Lazy<TSource> source,
            Func<TSource, Lazy<TNext>> selector,
            Func<TSource, TNext, TResult> resultSelector)
            => Unit(() => resultSelector(source.Value, selector(source.Value).Value));

        public static Lazy<TResult> Select<TSource, TResult>(
            this Lazy<TSource> source,
            Func<TSource, TResult> selector)
            => Unit(() => selector(source.Value));

        public static Lazy<TResult> Bind<TSource, TResult>(
            this Lazy<TSource> source,
            Func<TSource, Lazy<TResult>> selector)
            => Unit(() => selector(source.Value).Value);

        private static Lazy<T> Unit<T>(this Func<T> valueFactory)
            => new Lazy<T>(valueFactory);
    }

    static class FuncExtensions
    {
        private static Func<TResult> SelectManyF<TSource, TNext, TResult>(
            TSource source,
            Func<TSource, Func<TNext>> selector,
            Func<TSource, TNext, TResult> resultSelector)
            => () => resultSelector(source, selector(source)());

        public static Func<TResult> SelectMany<TSource, TNext, TResult>(
            this Func<TSource> source,
            Func<TSource, Func<TNext>> selector,
            Func<TSource, TNext, TResult> resultSelector)
            => SelectManyF(source(), selector, resultSelector);

        public static Func<TResult> Select<TSource, TResult>(
            this Func<TSource> source,
            Func<TSource, TResult> selector)
            => () => selector(source.Invoke());

        public static Func<TResult> Bind<TSource, TResult>(
            this Func<TSource> source,
            Func<TSource, Func<TResult>> selector)
            => () => selector(source.Invoke()).Invoke();

        private static Func<TSource, TResult> SelectManyF<TSource, TNext, TResult>(
            Func<TSource, Func<TNext>> selector,
            Func<TSource, TNext, TResult> resultSelector)
            => source => resultSelector(source, selector(source)());
    }
}