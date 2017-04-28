using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbcLeaves.Core;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace AbcLeaves.BasicMvcClient.Domain
{
    public class GoogleApisAuthManager : IGoogleApisAuthManager
    {
        private readonly GoogleOAuthOptions options;
        private readonly IAuthenticationManager authHelper;
        private readonly LeavesApiClient leavesApiClient;

        public GoogleApisAuthManager(
            IOptions<GoogleOAuthOptions> optionsAccessor,
            IAuthenticationManager authHelper,
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
            return await OperationFlow<ReturnUrlResult>
                .BeginWith(() => authHelper.GetAuthenticationPropertiesAsync())
                .ProceedWith(getAuthProps => {
                    getAuthProps.AuthProperties.Items.Add("returnUrl", returnUrl ?? "/");
                    return getAuthProps;
                })
                .ProceedWith(getAuthProps => authHelper.GetProtectedState(getAuthProps.AuthProperties))
                .ProceedWith(getState => BuildGoogleOAuthChallengeUrl(redirectUrl, getState.Value))
                .Return();
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
            return await OperationFlow<ReturnUrlResult>
                .BeginWith(() => authHelper.TestCrossSiteRequestForgery(state))
                .ProceedWithClosure(testCsrf => testCsrf
                    .ProceedWith(x => leavesApiClient.GrantGoogleApisAccess(code, redirectUrl))
                    .EndWith(x => {
                        var authProps = testCsrf.Current.AuthProperties;
                        var returnUrl = authProps.Items["returnUrl"] ?? "/";
                        return ReturnUrlResult.Success(returnUrl);
                    }))
                .Return();
        }

        private ReturnUrlResult BuildGoogleOAuthChallengeUrl(string redirectUrl, string state)
        {
            if (String.IsNullOrEmpty(redirectUrl))
            {
                throw new ArgumentNullException(nameof(redirectUrl));
            }
            if (String.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException(nameof(state));
            }
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
            return ReturnUrlResult.Success(challengeUrl);
        }
    }
}
