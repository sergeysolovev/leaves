using System;
using System.Collections.Generic;
using System.Linq;

namespace AbcLeaves.Core
{
    public class OperationResultBase : IOperationResult
    {
        public bool Succeeded { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public Dictionary<string, object> Details { get; protected set; } = new Dictionary<string, object>();

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
            if (details != null)
            {
                Details = details;
            }
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

        protected void MergeDetailsWith(Dictionary<string, object> moreDetails)
        {
            moreDetails.ToList().ForEach(x => Details[x.Key] = x.Value);
        }
    }
}
