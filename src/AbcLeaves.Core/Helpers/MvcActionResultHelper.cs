using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AbcLeaves.Core.Helpers
{
    public class MvcActionResultHelper : IMvcActionResultHelper
    {
        public MvcActionResultHelper() { }

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
            var returnUrlResult = operationResult as IReturnUrlOperationResult;
            if (returnUrlResult != null)
            {
                return Redirect(returnUrlResult.ReturnUrl);
            }
            return Ok(operationResult);
        }

        private IActionResult Forbidden(object value)
        {
            return new ObjectResult(value) {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }

        private IActionResult NotFound(object value)
        {
            return new NotFoundObjectResult(value);
        }

        private IActionResult BadRequest(object value)
        {
            return new BadRequestObjectResult(value);
        }

        private IActionResult Redirect(string url)
        {
            return new RedirectResult(url);
        }

        private IActionResult Ok(object value)
        {
            return new OkObjectResult(value);
        }
    }
}
