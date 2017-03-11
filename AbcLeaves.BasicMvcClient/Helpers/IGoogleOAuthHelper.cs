using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace AbcLeaves.BasicMvcClient.Helpers
{
    public interface IGoogleOAuthHelper
    {
        string BuildOfflineAccessChallengeUrl(string redirectUrl, string state);
    }
}
