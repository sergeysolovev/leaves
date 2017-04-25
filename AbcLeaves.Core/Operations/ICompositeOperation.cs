using System.Collections.Generic;

namespace AbcLeaves.Core
{
    public interface ICompositeOperation<TResult, TContext> : IOperation<TResult, TContext>
        where TResult : IOperationResult<TContext>
        where TContext : IOperationContext
    {
        void Add(IOperation<TResult, TContext> operation);
        void Clear();
        IReadOnlyCollection<IOperation<TResult, TContext>> Operations { get; }
    }
}
