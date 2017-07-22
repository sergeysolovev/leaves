using Microsoft.AspNetCore.Mvc;

namespace Leaves.Api.Common
{
    public static class UrlHelperExtensions
    {
        public static string Action<T>(this IUrlHelper helper, string action, string protocol)
            where T : Controller
        {
            var name = typeof(T).Name;
            string controllerName = name.EndsWith("Controller")
                ? name.Substring(0, name.Length - 10) : name;
            return helper.Action(action, controllerName, null, protocol);
        }
    }
}
