using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Operations
{
    /// <summary>
    /// Immutable, stateless
    /// </summary>
    internal struct Operation<TResult> : IOperation<TResult>
    {
        private readonly Func<Task<IContext<TResult>>> valueFactory;

        public Task<IContext<TResult>> ExecuteAsync()
            => valueFactory();

        public Operation(Func<Task<IContext<TResult>>> valueFactory)
            => this.valueFactory = Throw.IfNull(valueFactory, nameof(valueFactory));
    }
}