using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using ABC.Leaves.Api.Services;
using ABC.Leaves.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ABC.Leaves.Api.Domain;

namespace ABC.Leaves.Api.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IGoogleOAuthService googleAuthService;
        private readonly IUserManager appUserManager;

        public UserController(UserManager<AppUser> userManager,
            IGoogleOAuthService googleAuthService,
            IUserManager appUserManager)
        {
            this.userManager = userManager;
            this.appUserManager = appUserManager;
            this.googleAuthService = googleAuthService;
        }

        // POST api/user/
        [HttpPost]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Register()
        {
            var principal = HttpContext.User;
            var result = await appUserManager.CreateUserAsync(principal);
            if (!result.Succeeded)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }

        private static string GetJwtProperty(string token,
            Func<JwtSecurityToken, string> propertySelector)
        {
            JwtSecurityToken jwtToken;
            try
            {
                jwtToken = new JwtSecurityToken(token);
                return propertySelector(jwtToken);
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private async Task<bool> SaveUserToken(AppUser user, string loginProvider,
            string tokenName, string tokenValue)
        {
            var result = await userManager.SetAuthenticationTokenAsync(user,
                loginProvider, tokenName, tokenValue);
            return result.Succeeded;
        }

        [HttpGet("googleapis/check-access")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GoogleApisCheckAccess()
        {
            var principal = HttpContext.User;
            if (principal == null)
            {
                throw new InvalidOperationException();
            }
            var email = principal.FindFirstValue("email");
            if (email == null)
            {
                ModelState.AddModelError("message",
                    "Failed to retrieve an email claim from the authenticated claims principal");
                return new BadRequestObjectResult(ModelState);
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("message",
                    $"The user has not been registered. Use {Url.Action(nameof(Register))}");
                return new NotFoundObjectResult(ModelState);
            }

            var refreshToken = await userManager.GetAuthenticationTokenAsync(
                user, "Google", "refresh_token");

            // TODO: add check that refresh token is valid
            // and provides permissions to necessary scopes

            if (String.IsNullOrEmpty(refreshToken))
            {
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            return Ok();
        }

        // PATCH api/user/googleapis/grant-access?code=
        [HttpPatch("googleapis/grant-access")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GoogleApisGrantAccess(
            [FromQuery]string code,
            [FromQuery]string redirectUrl)
        {
            // get oauth code exchange response tokens:
            var exchangeResult = await googleAuthService.ExchangeCode(code, redirectUrl);
            if (!exchangeResult.Succeeded)
            {
                return BadRequest(exchangeResult.ErrorMessage);
            }
            var oauthIdToken = exchangeResult.IdToken;
            var oauthAccessToken = exchangeResult.AccessToken;
            var oauthRefreshToken = exchangeResult.RefreshToken;
            if (String.IsNullOrEmpty(oauthIdToken))
            {
                return BadRequest("Failed to retrieve oauth id_token");
            }
            if (String.IsNullOrEmpty(oauthAccessToken))
            {
                return BadRequest("Failed to retrieve oauth access_token");
            }
            if (String.IsNullOrEmpty(oauthRefreshToken))
            {
                return BadRequest("Failed to retrieve oauth refresh_token");
            }

            // get bearer token:
            var authContext = new AuthenticateContext("Bearer");
            await HttpContext.Authentication.AuthenticateAsync(authContext);
            if (authContext.Principal == null || authContext.Properties == null)
            {
                throw new InvalidOperationException();
            }
            var authProperties = new AuthenticationProperties(authContext.Properties);
            var bearerIdToken = authProperties.GetTokenValue("access_token");
            if (String.IsNullOrEmpty(bearerIdToken))
            {
                throw new InvalidOperationException();
            }

            // check auth code matches the identity
            var subjectFromBearer = GetJwtProperty(bearerIdToken, jwt => jwt.Subject);
            var subjectFromOAuth = GetJwtProperty(oauthIdToken, jwt => jwt.Subject);
            if (!String.Equals(subjectFromBearer, subjectFromOAuth, StringComparison.Ordinal))
            {
                return BadRequest("Authorization code doesn't match the authenticated identity");
            }

            // update the tokens:
            var principal = HttpContext.User;
            if (principal == null)
            {
                throw new InvalidOperationException();
            }
            var email = principal.FindFirstValue("email");
            if (email == null)
            {
                ModelState.AddModelError("message",
                    "Failed to retrieve an email claim from the authenticated claims principal");
                return new BadRequestObjectResult(ModelState);
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("message",
                    $"The user has not been registered. Use {Url.Action(nameof(Register))}");
                return new NotFoundObjectResult(ModelState);
            }
            var issuer = GetJwtProperty(oauthIdToken, jwt => jwt.Issuer);
            if (!await SaveUserToken(user, "Google", "refresh_token", oauthRefreshToken))
            {
                return BadRequest();
            }
            if (!await SaveUserToken(user, "Google", "access_token", oauthAccessToken))
            {
                return BadRequest();
            }
            return Ok();
        }

        // todo: deprecate
        // PATCH api/user/grant-admin-claims
        [HttpPatch("grant-admin-claims")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GrantAdminClaims()
        {
            var principal = HttpContext.User;
            if (principal == null)
            {
                throw new InvalidOperationException();
            }
            if (!userManager.SupportsUserEmail)
            {
                var message = "Backing user store does not support user emails";
                throw new NotSupportedException(message);
            }
            if (!userManager.SupportsUserClaim)
            {
                var message = "Backing user store does not support user claims";
                throw new NotSupportedException(message);
            }
            var email = principal.FindFirstValue("email");
            if (email == null)
            {
                ModelState.AddModelError("message",
                    "Failed to retrieve an email claim from the authenticated claims principal");
                return new BadRequestObjectResult(ModelState);
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("message",
                    $"The user has not been registered. Use {Url.Action(nameof(Register))}");
                return new NotFoundObjectResult(ModelState);
            }
            var userClaims = await userManager.GetClaimsAsync(user);
            if (userClaims.Any(c => c.Type == ClaimTypes.Role && c.Value == "admin"))
            {
                return Ok("Admin claims are already granted");
            }
            var adminRoleClaim = new Claim(ClaimTypes.Role, "admin");
            var identityResult = await userManager.AddClaimAsync(user, adminRoleClaim);
            if (!identityResult.Succeeded)
            {
                ModelState.AddModelError("message", "Failed to grant admin claims");
                return new BadRequestObjectResult(ModelState);
            }
            return Ok();
        }
    }
}
