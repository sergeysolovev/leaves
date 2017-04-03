using System.Collections.Generic;

namespace AbcLeaves.Core
{
    public interface IOperationResult
    {
        bool Succeeded { get; }
        string ErrorMessage { get; }
        Dictionary<string, object> Details { get; }
        void FailFrom(IOperationResult result);
    }
}
