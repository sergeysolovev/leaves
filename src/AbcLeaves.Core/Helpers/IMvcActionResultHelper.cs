using Microsoft.AspNetCore.Mvc;

namespace AbcLeaves.Core.Helpers
{
    public interface IMvcActionResultHelper
    {
        IActionResult FromOperationResult(IOperationResult operationResult);
    }
}
