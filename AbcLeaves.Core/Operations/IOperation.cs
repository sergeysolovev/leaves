using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public interface IOperation<TResult> : IOperation<TResult, DefaultOperationContext>
        where TResult : IOperationResult<DefaultOperationContext>
    {
        Task<TResult> ExecuteAsync();
    }

    public interface IOperation<TResult, TContext>
        where TResult : IOperationResult<TContext>
        where TContext : IOperationContext
    {
        Task<TResult> ExecuteAsync(TContext context);
    }
}
