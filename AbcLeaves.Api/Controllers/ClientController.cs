using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ABC.Leaves.Api.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ABC.Leaves.Api.Controllers
{
    [Route("mvcclient")]
    public class ClientController : Controller
    {
        private readonly GoogleOAuthOptions authOptions;
        private readonly IGoogleOAuthHelper googleOAuthHelper;
        private readonly HttpClient backchannel;
        private readonly PropertiesDataFormat stateDataFormat;

        public ClientController(IGoogleOAuthHelper googleOAuthHelper,
            IOptions<GoogleOAuthOptions> authOptionsAccessor,
            IDataProtectionProvider dataProtectionProvider,
            HttpClientHandler backchannelHttpHandler)
        {
            var dataProtector = dataProtectionProvider.CreateProtector(GetType().FullName);
            this.stateDataFormat = new PropertiesDataFormat(dataProtector);
            this.googleOAuthHelper = googleOAuthHelper;
            this.backchannel = new HttpClient(backchannelHttpHandler);
            this.authOptions = authOptionsAccessor.Value;
        }

        // GET /mvcclient/leave/approve
        [Authorize(ActiveAuthenticationSchemes = "GoogleOpenIdConnect")]
        [HttpGet("leave/approve/{id}")]
        public async Task<IActionResult> ApproveLeave(string id)
        {
            var authContext = new AuthenticateContext("GoogleOpenIdConnect");
            await HttpContext.Authentication.AuthenticateAsync(authContext);
            if (authContext.Principal == null || authContext.Properties == null)
            {
                throw new InvalidOperationException();
            }
            var authProperties = new AuthenticationProperties(authContext.Properties);
            var idToken = authProperties.GetTokenValue("id_token");
            if (String.IsNullOrEmpty(idToken))
            {
                return BadRequest();
            }

            // call api
            var apiUrl = $"http://localhost:5000/api/leaves/{id}/approve";
            var apiRequest = new HttpRequestMessage(new HttpMethod("PATCH"), apiUrl);
            apiRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            using (var apiResponse = await backchannel.SendAsync(apiRequest))
            {
                if (!apiResponse.IsSuccessStatusCode)
                {
                    return BadRequest(await apiResponse.Content.ReadAsStringAsync());
                }
                return Ok(await apiResponse.Content.ReadAsStringAsync());
            }
        }

        // GET /mvcclient/leave/approve
        [Authorize(ActiveAuthenticationSchemes = "GoogleOpenIdConnect")]
        [HttpGet("leave/decline/{id}")]
        public async Task<IActionResult> DeclineLeave(string id)
        {
            var authContext = new AuthenticateContext("GoogleOpenIdConnect");
            await HttpContext.Authentication.AuthenticateAsync(authContext);
            if (authContext.Principal == null || authContext.Properties == null)
            {
                throw new InvalidOperationException();
            }
            var authProperties = new AuthenticationProperties(authContext.Properties);
            var idToken = authProperties.GetTokenValue("id_token");
            if (String.IsNullOrEmpty(idToken))
            {
                return BadRequest();
            }

            // call api
            var apiUrl = $"http://localhost:5000/api/leaves/{id}/decline";
            var apiRequest = new HttpRequestMessage(new HttpMethod("PATCH"), apiUrl);
            apiRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            using (var apiResponse = await backchannel.SendAsync(apiRequest))
            {
                if (!apiResponse.IsSuccessStatusCode)
                {
                    return BadRequest(await apiResponse.Content.ReadAsStringAsync());
                }
                return Ok(await apiResponse.Content.ReadAsStringAsync());
            }
        }

        // GET /mvcclient/leave/apply
        [Authorize(ActiveAuthenticationSchemes = "GoogleOpenIdConnect")]
        [HttpGet("leave/apply")]
        public async Task<IActionResult> ApplyLeave([FromQuery]bool ensureGoogleApisAccess = true)
        {
            // get id_token
            var authContext = new AuthenticateContext("GoogleOpenIdConnect");
            await HttpContext.Authentication.AuthenticateAsync(authContext);
            if (authContext.Principal == null || authContext.Properties == null)
            {
                throw new InvalidOperationException();
            }
            var authProperties = new AuthenticationProperties(authContext.Properties);
            var idToken = authProperties.GetTokenValue("id_token");
            if (String.IsNullOrEmpty(idToken))
            {
                return BadRequest();
            }

            if (ensureGoogleApisAccess)
            {
                // use api to check access
                var apiUrl = "http://localhost:5000/api/user/googleapis/check-access";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
                var accessGranted = false;
                using (var response = await backchannel.SendAsync(request))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        accessGranted = true;
                    }
                    else if (response.StatusCode != HttpStatusCode.Forbidden)
                    {
                        return BadRequest();
                    }
                }

                // grant access
                if (!accessGranted)
                {
                    // request access with return url (incremental acc. to ggl)
                    var returnUrl = Url.Action(nameof(ApplyLeave), new { ensureGoogleApisAccess = false });
                    var redirectUrl = Url.Action(nameof(GoogleApisGrantAccess), new { returnUrl = returnUrl });
                    return Redirect(redirectUrl);
                }
            }

            // now that offline access is granted we can move on
            // and call api method for leave apply (start, end, access_token)
            // actually we don't need to send access_token, cz api will get
            // it on the backend side from refresh_token

            var apiUrl1 = "http://localhost:5000/api/leaves";
            var request1 = new HttpRequestMessage(HttpMethod.Post, apiUrl1);
            request1.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            var leave = new {
                Start = DateTime.Now,
                End = DateTime.UtcNow.AddHours(1)
            };
            var leaveJson = JsonConvert.SerializeObject(leave,
                new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
            var httpContent = new StringContent(leaveJson, Encoding.UTF8, "application/json");
            request1.Content = httpContent;
            using (var response = await backchannel.SendAsync(request1))
            {
                return new ObjectResult(await response.Content.ReadAsStringAsync())
                {
                    StatusCode = (int)response.StatusCode
                };
            }
        }

        // GET /mvcclient/register
        [Authorize(ActiveAuthenticationSchemes = "GoogleOpenIdConnect")]
        [HttpGet("register")]
        public async Task<IActionResult> RegisterUser()
        {
            // get id_token
            var authContext = new AuthenticateContext("GoogleOpenIdConnect");
            await HttpContext.Authentication.AuthenticateAsync(authContext);
            if (authContext.Principal == null || authContext.Properties == null)
            {
                throw new InvalidOperationException();
            }
            var authProperties = new AuthenticationProperties(authContext.Properties);
            var idToken = authProperties.GetTokenValue("id_token");
            if (String.IsNullOrEmpty(idToken))
            {
                return BadRequest();
            }

            // use api:
            var apiUrl = "http://localhost:5000/api/user";
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            using (var apiResponse = await backchannel.SendAsync(request))
            {
                if (!apiResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("error",
                        await apiResponse.Content.ReadAsStringAsync());
                    return BadRequest(ModelState);
                }
                return Ok($"The user succesfully registered");
            }
        }

        // GET /mvcclient/googleapis/grant-access
        [Authorize(ActiveAuthenticationSchemes = "GoogleOpenIdConnect")]
        [HttpGet("googleapis/grant-access")]
        public async Task<IActionResult> GoogleApisGrantAccess([FromQuery]string returnUrl = null)
        {
            var authContext = new AuthenticateContext("GoogleOpenIdConnect");
            await HttpContext.Authentication.AuthenticateAsync(authContext);
            if (authContext.Principal == null || authContext.Properties == null)
            {
                throw new InvalidOperationException();
            }
            var authProperties = new AuthenticationProperties(authContext.Properties);
            authProperties.Items.Add("returnUrl", returnUrl ?? "/");
            var state = stateDataFormat.Protect(authProperties);
            var acceptAction = nameof(GoogleApisGrantAccessAccept);
            var redirectUrl = Url.Action(acceptAction, null, null, Request.Scheme);
            var challengeUrl = googleOAuthHelper.BuildOfflineAccessChallengeUrl(redirectUrl, state);
            return Redirect(challengeUrl);
        }

        // GET /mvcclient/googleapis/grant-access/accept
        [HttpGet("googleapis/grant-access/accept")]
        [Authorize(ActiveAuthenticationSchemes = "GoogleOpenIdConnect")]
        public async Task<IActionResult> GoogleApisGrantAccessAccept(
            [FromQuery]string code = null,
            [FromQuery]string state = null,
            [FromQuery]string error = null,
            [FromQuery]string scope = null)
        {
            if (error != null)
            {
                return BadRequest($"Failed to grant access to Google Apis: {error}");
            }
            if (code == null || state == null)
            {
                return BadRequest();
                // log: code and state is not presented
            }
            // OAuth 10.12 CSRF
            var stateAuthProperties = stateDataFormat.Unprotect(state);
            if (stateAuthProperties == null)
            {
                return BadRequest();
                // log: state is invalid
            }
            string idTokenFromState = stateAuthProperties.GetTokenValue("id_token");
            if (String.IsNullOrEmpty(idTokenFromState))
            {
                return BadRequest();
                // log: can not retrieve id_token from state
            }
            var authContext = new AuthenticateContext("GoogleOpenIdConnect");
            await HttpContext.Authentication.AuthenticateAsync(authContext);
            if (authContext.Principal == null || authContext.Properties == null)
            {
                throw new InvalidOperationException();
            }
            var cookiesAuthProperties = new AuthenticationProperties(authContext.Properties);
            string idTokenFromCookie = cookiesAuthProperties.GetTokenValue("id_token");
            if (String.IsNullOrEmpty(idTokenFromCookie))
            {
                return BadRequest();
                // log: can not retrieve id_token from cookies
            }
            if (!String.Equals(idTokenFromState, idTokenFromCookie, StringComparison.Ordinal))
            {
                return BadRequest();
                // log: id_token from cookies and state don't match
            }

            // all checks passed
            // use the backchannel to call the API
            // (todo: move to a service)
            var apiUrl = QueryHelpers.AddQueryString(
                uri: "http://localhost:5000/api/user/googleapis/grant-access",
                queryString: new Dictionary<string, string> {
                    ["code"] = code,
                    ["redirectUrl"] = Url.Action(null, null, null, Request.Scheme)
                });
            var apiRequest = new HttpRequestMessage(new HttpMethod("PATCH"), apiUrl);
            apiRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idTokenFromState);
            using (var apiResponse = await backchannel.SendAsync(apiRequest))
            {
                if (!apiResponse.IsSuccessStatusCode)
                {
                    return BadRequest(await apiResponse.Content.ReadAsStringAsync());
                }
                var returnUrl = stateAuthProperties.Items["returnUrl"] ?? "/";
                return Redirect(returnUrl);
            }
        }

        [AllowAnonymous]
        [HttpGet("auth/signout")]
        public async Task<IActionResult> Signout()
        {
            await HttpContext.Authentication.SignOutAsync("GoogleOpenIdConnectCookies");
            return Ok("Successfully signed out");
        }
    }
}
