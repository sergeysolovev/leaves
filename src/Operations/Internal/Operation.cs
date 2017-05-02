using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Operations
{
    internal class Operation<T> : IOperation<T>
    {
        private readonly Func<Task<IContext<T>>> valueFactory;

        public Task<IContext<T>> ExecuteAsync()
            => valueFactory();

        public Operation(Func<Task<IContext<T>>> valueFactory)
            => this.valueFactory = Throw.IfNull(valueFactory, nameof(valueFactory));
    }
}