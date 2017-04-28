using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AbcLeaves.Api.Helpers
{
    public interface IBackChannelHelper
    {
        Task<Dictionary<string, object>> GetResponseDetailsAsync(HttpResponseMessage response);
    }
}
