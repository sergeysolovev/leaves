using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Operations
{
    public static class OperationExtensions
    {
        public static TaskAwaiter<IResult<T>> GetAwaiter<T>(
            this IOperation<T> self)
            => self.ExecuteAsync().GetAwaiter();
    }
}