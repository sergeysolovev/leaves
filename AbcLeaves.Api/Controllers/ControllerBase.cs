using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AbcLeaves.Api.Controllers
{
    public class ControllerBase : Controller
    {
        public IActionResult Forbidden(object value)
        {
            return new ObjectResult(value) {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }

        public IActionResult FromOperationResult(IOperationResult operationResult)
        {
            if (!operationResult.Succeeded)
            {
                var forbiddenResult = operationResult as IForbiddenOperationResult;
                if (forbiddenResult != null && forbiddenResult.IsForbidden)
                {
                    return Forbidden(operationResult);
                }
                var notFoundResult = operationResult as INotFoundOperationResult;
                if (notFoundResult != null && notFoundResult.NotFound)
                {
                    return NotFound(operationResult);
                }
                return BadRequest(operationResult);
            }
            return Ok(operationResult);
        }
    }
}
