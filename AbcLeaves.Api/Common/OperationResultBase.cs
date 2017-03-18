using System;
using System.Collections.Generic;

namespace AbcLeaves.Api
{
    public class OperationResultBase : IOperationResult
    {
        public bool Succeeded { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public Dictionary<string, object> Details { get; protected set; }

        public OperationResultBase()
        {
        }

        protected OperationResultBase(bool succeeded)
        {
            Succeeded = succeeded;
        }

        protected OperationResultBase(string error, Dictionary<string, object> details)
        {
            ErrorMessage = error;
            Details = details;
        }

        protected OperationResultBase(IOperationResult result)
        {
            FailFromInternal(result);
        }

        void IOperationResult.FailFrom(IOperationResult result)
        {
            FailFromInternal(result);
        }

        protected void FailFromInternal(IOperationResult result)
        {
            if (result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Can not fail from successful result");
            }
            ErrorMessage = result.ErrorMessage;
            if (result.Details != null)
            {
                Details = new Dictionary<string, object>(result.Details);
            }
        }
    }
}
