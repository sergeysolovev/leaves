using System;
using System.Threading.Tasks;

namespace Operations
{
    public static class OperationBuilderExtensions
    {
        public static IOperation<TR> BuildWith<TS, TR>(
            this IOperationBuilder<TS> source,
            IOperationService<TS, TR> service)
            => source.Build().Bind(service);

        public static IOperation<TR> BuildWith<TS, TR>(
            this IOperationBuilder<TS> source,
            Func<TS, Task<IContext<TR>>> closure)
            => source.Build().Bind(closure);

        public static IOperation<TR> BuildWith<TS, TR>(
            this IOperationBuilder<TS> source,
            Func<TS, IContext<TR>> closure)
            => source.Build().Bind(closure);
    }
}