using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Operations
{
    public static class Operation
    {
        public static IOperation<TR> Return<TR>(Func<Task<IContext<TR>>> valueFactory)
            => new Operation<TR>(valueFactory);

        public static IOperation<TR> Return<TR>(Func<IContext<TR>> valueFactory)
            => new Operation<TR>(() => Wrap(valueFactory()));

        public static IOperation<TR> Return<TR>(Func<TR> valueFactory)
            => Return(() => Context.Succeed(valueFactory()));

        public static IOperation<TR> Return<TR>(TR value)
            => Return(() => Context.Succeed(value));

        public static IOperation<TR> ReturnDefault<TR>()
            => Return(() => Context.Default<TR>());

        public static IOperation<TR> ReturnNew<TR>() where TR : new()
            => Return(new TR());

        public static IOperation<TR> Where<TR>(
            this IOperation<TR> source,
            Func<TR, bool> predicate)
            => Return(() => source.ExecuteAsync().Bind(x =>
                x.Succeeded && predicate(x.Result) ?
                    x.Wrap() :
                    Context.FailFrom(x).Wrap()));

        public static IOperation<TR> Select<TS, TR>(
            this IOperation<TS> source,
            Func<TS, TR> selector)
            => Return(() => source.ExecuteAsync().Bind(x =>
                x.Succeeded ?
                    Context.Succeed(selector(x.Result)).Wrap() :
                    Context.FailFrom<TS, TR>(x).Wrap()));

        public static IOperation<TR> SelectMany<TS, TM, TR>(
            this IOperation<TS> source,
            Func<TS, IOperation<TM>> closure,
            Func<TS, TM, TR> selector)
            => Bind(source, Compose(closure, selector));

        public static IOperation<TR> Bind<TS, TR>(
            this IOperation<TS> source,
            Func<TS, IOperation<TR>> closure)
            => Return(() => source.ExecuteAsync().Bind(x =>
                x.Succeeded ?
                    closure(x.Result).ExecuteAsync() :
                    Context.FailFrom<TS, TR>(x).Wrap()));

        public static IOperation<TR> Bind<TS, TR>(
            this IOperation<TS> source,
            IOperationService<TS, TR> service)
            => Bind(source, service.ToClosure());

        public static IOperation<TR> Bind<TR>(
            this IOperation<TR> source,
            IOperationService<TR> service)
            => Bind(source, service.ToClosure());

        public static IOperation<TR> Bind<TS, TR>(
            this IOperation<TS> source,
            Func<TS, Task<IContext<TR>>> closure)
            => Bind(source, OperationService.Return(closure));

        public static IOperation<TR> Bind<TS, TR>(
            this IOperation<TS> source,
            Func<TS, IContext<TR>> closure)
            => Bind(source, OperationService.Return(closure));

        private static Func<TS, IOperation<TR>> Compose<TS, TM, TR>(
            Func<TS, IOperation<TM>> closure,
            Func<TS, TM, TR> selector)
            => x => Return(() => closure(x).ExecuteAsync().Bind(y =>
                y.Succeeded ?
                    Context.Succeed(selector(x, y.Result)).Wrap() :
                    Context.FailFrom<TM, TR>(y).Wrap()));

        private static Task<TR> Wrap<TR>(this TR value)
            => Task.FromResult(value);

        private static async Task<TR> Bind<TS, TR>(
            this Task<TS> source,
            Func<TS, Task<TR>> selector)
            => await selector(await source);
    }
}