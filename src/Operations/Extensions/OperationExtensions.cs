using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Operations
{
    public static class OperationExtensions
    {
        public static TaskAwaiter<IContext<TResult>> GetAwaiter<TResult>(
            this IOperation<TResult> source)
            => source.ExecuteAsync().GetAwaiter();
    }
}