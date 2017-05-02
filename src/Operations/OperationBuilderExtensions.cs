using System;
using System.Threading.Tasks;

namespace Operations
{
    public static class OperationBuilderExtensions
    {
        public static void Inject<T>(
            this IOperationBuilder<T> source,
            Func<T, IContext<T>> closure)
            => source.Inject(OperationService.Return(closure));

        public static void Inject<T>(
            this IOperationBuilder<T> source,
            Func<T, Task<IContext<T>>> closure)
            => source.Inject(OperationService.Return(closure));
    }
}