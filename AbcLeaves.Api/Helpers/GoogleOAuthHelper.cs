using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace ABC.Leaves.Api.Helpers
{
    public class GoogleOAuthHelper : IGoogleOAuthHelper
    {
        private readonly GoogleOAuthOptions options;

        public GoogleOAuthHelper(IOptions<GoogleOAuthOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            this.options = optionsAccessor.Value;
        }

        public string BuildOfflineAccessChallengeUrl(string redirectUrl, string state)
        {
            if (String.IsNullOrEmpty(redirectUrl))
            {
                throw new ArgumentNullException(nameof(redirectUrl));
            }
            if (String.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            return QueryHelpers.AddQueryString(
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
        }
    }
}
