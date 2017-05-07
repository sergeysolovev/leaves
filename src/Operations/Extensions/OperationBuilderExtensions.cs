using System;
using System.Threading.Tasks;

namespace Operations
{
    public static class OperationBuilderExtensions
    {
        public static IOperation<T> Build<T>(
            this IOperationBuilder<T> source)
            where T : new()
            => source.BuildWith(new T());

        public static IOperation<TR> BuildWith<TS, TR>(
            this IOperationBuilder<TS> source,
            TS injection,
            IOperationService<TS, TR> service)
            => source.BuildWith(injection).Bind(service);

        public static IOperation<TR> BuildWith<TS, TR>(
            this IOperationBuilder<TS> source,
            IOperationService<TS, TR> service)
            where TS : new()
            => source.Build().Bind(service);

        public static IOperation<TR> BuildWith<TS, TR>(
            this IOperationBuilder<TS> source,
            TS injection,
            Func<TS, Task<IContext<TR>>> closure)
            => source.BuildWith(injection).Bind(closure);

        public static IOperation<TR> BuildWith<TS, TR>(
            this IOperationBuilder<TS> source,
            Func<TS, Task<IContext<TR>>> closure)
            where TS : new()
            => source.Build().Bind(closure);

        public static IOperation<TR> BuildWith<TS, TR>(
            this IOperationBuilder<TS> source,
            TS injection,
            Func<TS, IContext<TR>> closure)
            => source.BuildWith(injection).Bind(closure);

        public static IOperation<TR> BuildWith<TS, TR>(
            this IOperationBuilder<TS> source,
            Func<TS, IContext<TR>> closure)
            where TS : new()
            => source.Build().Bind(closure);
    }
}