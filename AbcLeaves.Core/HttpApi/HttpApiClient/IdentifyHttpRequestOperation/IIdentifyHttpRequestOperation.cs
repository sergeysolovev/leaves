using System.Net.Http;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public interface IIdentifyHttpRequestOperation
    {
        Task<IdentifyHttpRequestResult> ExecuteAsync(HttpRequestMessage request);
    }
}
