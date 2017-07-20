using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AbcLeaves.Api.Operations
{
    public static class OperationResultExtensions
    {
        public static IActionResult ToMvcActionResult(this IOperationResult source)
        {
            switch (source)
            {
                case IVerifyAccessResult x when x.IsForbidden:
                    return Forbidden(source);
                case IFindResult x when x.NotFound:
                    return NotFound(source);
                case var x when !x.Succeeded:
                    return BadRequest(source);
                case IReturnUrlResult x:
                    return Redirect(x.ReturnUrl);
                default:
                    return Ok(source);
            }
        }

        public static async Task<IActionResult> ToMvcActionResultAsync<TOperationResult>(
            this Task<TOperationResult> source) where TOperationResult : IOperationResult
        {
            return ToMvcActionResult(await source);
        }

        private static IActionResult Forbidden(object value)
            => new ObjectResult(value) {
                StatusCode = (int)HttpStatusCode.Forbidden
            };

        private static IActionResult NotFound(object value)
            => new NotFoundObjectResult(value);

        private static IActionResult BadRequest(object value)
            => new BadRequestObjectResult(value);

        private static IActionResult Redirect(string url)
            => new RedirectResult(url);

        private static IActionResult Ok(object value)
            => new OkObjectResult(value);
    }
}
