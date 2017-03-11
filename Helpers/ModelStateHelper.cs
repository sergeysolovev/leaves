using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace ABC.Leaves.Api.Helpers
{
    public class ModelStateHelper : IModelStateHelper
    {
        public ModelStateHelper() { }

        public Dictionary<string, object> GetValidationErrors(ModelStateDictionary modelState)
        {
            return modelState
                .Where(x => x.Value.ValidationState == ModelValidationState.Invalid)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors
                        .Select(e => GetErrorMessage(e))
                        .Where(e => e != null) as object);
        }

        private static string GetErrorMessage(ModelError error)
        {
            if (!String.IsNullOrEmpty(error.ErrorMessage))
            {
                return error.ErrorMessage;
            }
            if (!String.IsNullOrEmpty(error.Exception?.Message))
            {
                return error.Exception.Message;
            }
            return null;
        }
    }
}
