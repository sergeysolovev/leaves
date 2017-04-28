using System;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public static class OperationFlow<TReturn> where TReturn : IOperationResult
    {
        public static async Task<OperationFlowState<TReturn>> BeginWith(
            Func<Task<TReturn>> operation)
        {
            return await BeginWith<TReturn>(operation).Fold();
        }

        public static async Task<OperationFlowState<TReturn>> BeginWith(
            Func<TReturn> operation)
        {
            return await BeginWith<TReturn>(operation).Fold();
        }

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
            return await BeginWith(() => Task.FromResult(operation.Invoke()));
        }
    }
}
