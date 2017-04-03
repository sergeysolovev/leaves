using System;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public static class Operation<TReturn>
        where TReturn : IOperationResult
    {
        public static async Task<OperationFlowState<TReturn, TCurrent>> BeginWith<TCurrent>(
            Func<Task<TCurrent>> operation
        )
            where TCurrent : IOperationResult
        {
            return new OperationFlowState<TReturn, TCurrent>(await operation.Invoke());
        }

        public static async Task<OperationFlowState<TReturn, TCurrent>> BeginWith<TCurrent>(
            Func<TCurrent> operation
        )
            where TCurrent : IOperationResult
        {
            var operationAsync = Task.FromResult(operation.Invoke());
            return new OperationFlowState<TReturn, TCurrent>(await operationAsync);
        }
    }
}