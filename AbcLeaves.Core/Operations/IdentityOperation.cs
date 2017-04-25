using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public class IdentityOperation<TResult, TContext> : Operation<TResult, TContext>
        where TResult : IOperationResult<TContext>, new()
        where TContext : IOperationContext
    {
        protected IdentityOperation() {}

        public static IdentityOperation<TResult, TContext> Create()
        {
            return new IdentityOperation<TResult, TContext>();
        }

        public override async Task<TResult> ExecuteAsync(TContext context)
        {
            var result = new TResult();
            result.Succeed(context);
            return await Task.FromResult(result);
        }
    }
}