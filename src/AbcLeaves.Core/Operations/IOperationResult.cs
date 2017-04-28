using System.Collections.Generic;

namespace AbcLeaves.Core
{
    // todo: implement non-generic context for IOperationResult
    public interface IOperationResult
    {
        bool Succeeded { get; }
        string ErrorMessage { get; }
        Dictionary<string, object> Details { get; }
        void FailFrom(IOperationResult result);
    }

    public interface IOperationResult<TContext> : IOperationResult
        where TContext : IOperationContext
    {
        TContext Context { get; set; }
        void Succeed(TContext Context);
    }
}
