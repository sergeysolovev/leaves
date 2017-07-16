using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbcLeaves.Core;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace AbcLeaves.BasicMvcClient.Domain
{
    public class GoogleApisAuthManager
    {
        private readonly GoogleOAuthOptions options;
        private readonly AuthenticationManager authHelper;
        private readonly LeavesApiClient leavesApiClient;

        public GoogleApisAuthManager(
            IOptions<GoogleOAuthOptions> optionsAccessor,
            AuthenticationManager authHelper,
            LeavesApiClient leavesApiClient)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }
            if (authHelper == null)
            {
                throw new ArgumentNullException(nameof(authHelper));
            }
            if (leavesApiClient == null)
            {
                throw new ArgumentNullException(nameof(leavesApiClient));
            }
            this.options = optionsAccessor.Value;
            this.authHelper = authHelper;
            this.leavesApiClient = leavesApiClient;
        }

        public async Task<ReturnUrlResult> GetChallengeUrl(
            string redirectUrl,
            string returnUrl)
        {
            var authProps = await authHelper.GetAuthenticationPropertiesAsync();
            authProps.Items.Add("returnUrl", returnUrl ?? "/");
            var state = authHelper.GetProtectedState(authProps);
            return BuildGoogleOAuthChallengeUrl(redirectUrl, state);
        }

        public async Task<ReturnUrlResult> HandleOAuthExchangeCode(
            string code,
            string state,
            string error,
            string redirectUrl)
        {
            if (error != null)
            {
                return ReturnUrlResult.Fail($"Failed to grant access to Google Apis: {error}");
            }
            if (code == null)
            {
                return ReturnUrlResult.Fail($"{nameof(code)} parameter is required");
            }
            if (state == null)
            {
                return ReturnUrlResult.Fail($"{nameof(state)} parameter is required");
            }

            var authPropsResult = await authHelper.TestCrossSiteRequestForgery(state);
            if (!authPropsResult.Succeeded)
            {
                return ReturnUrlResult.Fail("Failed to test CSRF");
            }

            var x = await leavesApiClient.GrantGoogleApisAccess(code, redirectUrl);
            var authProps = authPropsResult.AuthProperties;
            var returnUrl = authProps.Items["returnUrl"] ?? "/";
            return ReturnUrlResult.Succeed(returnUrl);
        }

        private ReturnUrlResult BuildGoogleOAuthChallengeUrl(string redirectUrl, string state)
        {
            Throw.IfNullOrEmpty(redirectUrl, nameof(redirectUrl));
            Throw.IfNullOrEmpty(state, nameof(state));

            var challengeUrl = QueryHelpers.AddQueryString(
                uri: options.AuthUri,
                queryString: new Dictionary<string, string> {
                    ["response_type"] = "code",
                    ["client_id"] = options.ClientId,
                    ["scope"] = String.Join(" ", options.Scopes),
                    ["redirect_uri"] = redirectUrl,
                    ["access_type"] = "offline",
                    ["prompt"] = "consent",
                    ["include_granted_scopes"] = "true",
                    ["state"] = state
                });

            return ReturnUrlResult.Succeed(challengeUrl);
        }
    }
}
