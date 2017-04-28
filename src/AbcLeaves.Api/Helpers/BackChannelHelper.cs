using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace AbcLeaves.Api.Helpers
{
    public class BackChannelHelper : IBackChannelHelper
    {
        public BackChannelHelper() { }

        public async Task<Dictionary<string, object>> GetResponseDetailsAsync(
            HttpResponseMessage response)
        {
            return new Dictionary<string, object>
            {
                ["status"] = response.StatusCode,
                ["headers"] = response.Headers.ToString(),
                ["body"] = await response.Content.ReadAsStringAsync()
            };
        }
    }
}
