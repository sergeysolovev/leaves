using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Leaves.Utils;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Leaves.MvcClient.Helpers
{
    public class GoogleOAuthHelper
    {
        private readonly GoogleOAuthOptions options;

        public GoogleOAuthHelper(IOptions<GoogleOAuthOptions> options)
        {
            this.options = Throw.IfNull(options, nameof(options)).Value;
        }

        public string BuildChallengeUrl(string redirectUrl, string state)
        {
            Throw.IfNullOrEmpty(redirectUrl, nameof(redirectUrl));
            Throw.IfNullOrEmpty(state, nameof(state));

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
