using System;
using System.Net;
using System.Threading.Tasks;
using Leaves.Client.DataContracts;
using Leaves.Client.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leaves.Client.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public IActionResult Index() => Redirect("/leaves");

        [Route("idtoken")]
        public IActionResult IdToken() => Redirect("/auth/idtoken");
    }
}
