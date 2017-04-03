using System.Collections.Generic;

namespace AbcLeaves.Core
{
    public class ModelValidationResult : OperationResultBase
    {
        private const string ValidationErrorMessage = "One or more errors occurred during validation";

        public ModelValidationResult() : base() {}

        protected ModelValidationResult(bool succeeded) : base(succeeded) {}

        protected ModelValidationResult(Dictionary<string, object> validationErrors)
            : base(ValidationErrorMessage, validationErrors)
        {
        }

        public static ModelValidationResult Success
            => new ModelValidationResult(true);

        public static ModelValidationResult Fail(Dictionary<string, object> validationErrors)
            => new ModelValidationResult(validationErrors);
    }
}
