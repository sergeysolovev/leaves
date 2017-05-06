using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Operations
{
    /// <summary>
    /// Immutable, stateless
    /// </summary>
    internal struct OperationService<TSource, TResult> : IOperationService<TSource, TResult>
    {
        private readonly Func<TSource, IOperation<TResult>> closure;

        internal OperationService(Func<TSource, IOperation<TResult>> closure)
            => this.closure = Throw.IfNull(closure, nameof(closure));

        public IOperation<TResult> Inject(TSource value)
            => closure(value);
    }

    /// <summary>
    /// Immutable, stateless
    /// </summary>
    internal struct OperationService<TResult> : IOperationService<TResult>
    {
        private readonly Func<TResult, IOperation<TResult>> closure;

        internal OperationService(Func<TResult, IOperation<TResult>> closure)
            => this.closure = Throw.IfNull(closure, nameof(closure));

        public IOperation<TResult> Inject(TResult value)
            => closure(value);
    }
}