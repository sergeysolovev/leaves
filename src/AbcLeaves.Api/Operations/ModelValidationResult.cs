using System.Collections.Generic;

namespace AbcLeaves.Api.Operations
{
    public class ModelValidationResult : OperationResult
    {
        public ModelValidationResult() : base() { }

        public ModelValidationResult(IDictionary<string, object> validationErrors)
            : base("One or more errors occurred during validation")
        {
            Failure.Add("errors", validationErrors);
        }

        public static ModelValidationResult Succeeed()
            => new ModelValidationResult();

        public static ModelValidationResult Fail(Dictionary<string, object> validationErrors)
            => new ModelValidationResult(validationErrors);
    }
}
