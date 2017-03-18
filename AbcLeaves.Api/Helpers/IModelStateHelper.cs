using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AbcLeaves.Api.Helpers
{
    public interface IModelStateHelper
    {
        Dictionary<string, object> GetValidationErrors(ModelStateDictionary modelState);
    }
}
