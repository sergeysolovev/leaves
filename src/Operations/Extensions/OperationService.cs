using System;
using System.Threading.Tasks;

namespace Operations
{
    public static class OperationService
    {
        public static IOperationService<TS, TR> Return<TS, TR>(
            Func<TS, IOperation<TR>> closure)
            => new OperationService<TS, TR>(closure);

        public static IOperationService<TR> Return<TR>(
            Func<TR, IOperation<TR>> closure)
            => new OperationService<TR>(closure);

        public static IOperationService<TS, TR> Return<TS, TR>(
            Func<TS, Task<IContext<TR>>> closure)
            => Return<TS, TR>(x => Operation.Return(() => closure(x)));

        public static IOperationService<TR> Return<TR>(
            Func<TR, Task<IContext<TR>>> closure)
            => Return<TR>(x => Operation.Return(() => closure(x)));

        public static IOperationService<TS, TR> Return<TS, TR>(
            Func<TS, IContext<TR>> closure)
            => Return<TS, TR>(x => Operation.Return(() => closure(x)));

        public static IOperationService<TR> Return<TR>(
            Func<TR, IContext<TR>> closure)
            => Return<TR>(x => Operation.Return(() => closure(x)));

        public static IOperationService<TR> Id<TR>()
            => Return<TR>(x => Operation.Return(x));

        public static Func<TS, IOperation<TR>> ToClosure<TS, TR>(
            this IOperationService<TS, TR> source)
            => x => source.Inject(x);

        public static Func<TR, IOperation<TR>> ToClosure<TR>(
            this IOperationService<TR> source)
            => x => source.Inject(x);

        public static IOperationService<TS, TR> AddInner<TS, TM, TR>(
            this IOperationService<TM, TR> source,
            IOperationService<TS, TM> inner)
            => Return<TS, TR>(x => inner.Inject(x).Bind(source));

        public static IOperationService<TR> AddInner<TR>(
            this IOperationService<TR> source,
            IOperationService<TR> inner)
            => Return<TR>(x => inner.Inject(x).Bind(source));

        public static IOperationService<TS, TR> AddInner<TS, TM, TR>(
            this IOperationService<TM, TR> source,
            Func<TS, IOperation<TM>> inner)
            => Return<TS, TR>(x => inner(x).Bind(ToClosure(source)));

        public static IOperationService<TR> AddInner<TR>(
            this IOperationService<TR> source,
            Func<TR, IOperation<TR>> inner)
            => Return<TR>(x => inner(x).Bind(ToClosure(source)));

        public static IOperationService<TS, TR> AddInner<TS, TM, TR>(
            this IOperationService<TM, TR> source,
            Func<TS, Task<IContext<TM>>> inner)
            => AddInner(source, Return(inner));

        public static IOperationService<TR> AddInner<TR>(
            this IOperationService<TR> source,
            Func<TR, Task<IContext<TR>>> inner)
            => AddInner(source, Return<TR>(inner));

        public static IOperationService<TS, TR> AddInner<TS, TM, TR>(
            this IOperationService<TM, TR> source,
            Func<TS, IContext<TM>> inner)
            => AddInner(source, Return(inner));

        public static IOperationService<TR> AddInner<TR>(
            this IOperationService<TR> source,
            Func<TR, IContext<TR>> inner)
            => AddInner(source, Return<TR>(inner));

        public static IOperationService<TS, TR> AddOuter<TS, TM, TR>(
            this IOperationService<TS, TM> source,
            IOperationService<TM, TR> outer)
            => Return<TS, TR>(x => source.Inject(x).Bind(outer));

        public static IOperationService<TR> AddOuter<TR>(
            this IOperationService<TR> source,
            IOperationService<TR> outer)
            => Return<TR>(x => source.Inject(x).Bind(outer));

        public static IOperationService<TS, TR> AddOuter<TS, TM, TR>(
            this IOperationService<TS, TM> source,
            Func<TM, IOperation<TR>> outer)
            => Return<TS, TR>(x => source.Inject(x).Bind(outer));

        public static IOperationService<TR> AddOuter<TR>(
            this IOperationService<TR> source,
            Func<TR, IOperation<TR>> outer)
            => Return<TR>(x => source.Inject(x).Bind(outer));

        public static IOperationService<TS, TR> AddOuter<TS, TM, TR>(
            this IOperationService<TS, TM> source,
            Func<TM, Task<IContext<TR>>> inner)
            => AddOuter(source, Return(inner));

        public static IOperationService<TR> AddOuter<TR>(
            this IOperationService<TR> source,
            Func<TR, Task<IContext<TR>>> inner)
            => AddOuter(source, Return<TR>(inner));

        public static IOperationService<TS, TR> AddOuter<TS, TM, TR>(
            this IOperationService<TS, TM> source,
            Func<TM, IContext<TR>> inner)
            => AddOuter(source, Return(inner));

        public static IOperationService<TR> AddOuter<TR>(
            this IOperationService<TR> source,
            Func<TR, IContext<TR>> inner)
            => AddOuter(source, Return<TR>(inner));
    }
}