using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Operations
{
    internal class Operation<T> : IOperation<T>
    {
        private readonly Func<Task<IResult<T>>> executeAsync;

        public Task<IResult<T>> ExecuteAsync()
            => executeAsync();

        public Operation(Func<Task<IResult<T>>> valueFactory)
            => executeAsync = Throw.IfNull(valueFactory, nameof(valueFactory));
    }
}