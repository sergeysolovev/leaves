using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ABC.Leaves.Api.Helpers
{
    public interface IModelStateHelper
    {
        Dictionary<string, object> GetValidationErrors(ModelStateDictionary modelState);
    }
}
