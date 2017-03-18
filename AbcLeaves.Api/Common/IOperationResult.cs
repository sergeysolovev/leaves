using System.Collections.Generic;

namespace AbcLeaves.Api
{
    public interface IOperationResult
    {
        bool Succeeded { get; }
        string ErrorMessage { get; }
        Dictionary<string, object> Details { get; }
        void FailFrom(IOperationResult result);
    }
}
