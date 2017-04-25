using System;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public abstract class Operation<TResult> : Operation<TResult, DefaultOperationContext>, IOperation<TResult>
        where TResult : IOperationResult<DefaultOperationContext>
    {
        public Task<TResult> ExecuteAsync() => ExecuteAsync(new DefaultOperationContext());
    }

    public abstract class Operation<TResult, TContext> : IOperation<TResult, TContext>
        where TResult : IOperationResult<TContext>
        where TContext : IOperationContext
    {
        public abstract Task<TResult> ExecuteAsync(TContext context);

        public async Task<OperationFlowState<TResult>> Begin(TContext context)
        {
            return await BeginWith(() => ExecuteAsync(context)).Fold();
        }

        public async Task<OperationFlowState<TResult, TCurrent>> BeginWith<TCurrent>(
            Func<TCurrent> operation
        )
            where TCurrent : IOperationResult
        {
            return await OperationFlow<TResult>.BeginWith(operation);
        }

        public async Task<OperationFlowState<TResult, TCurrent>> BeginWith<TCurrent>(
            Func<Task<TCurrent>> operation
        )
            where TCurrent : IOperationResult
        {
            return await OperationFlow<TResult>.BeginWith(operation);
        }
    }
}