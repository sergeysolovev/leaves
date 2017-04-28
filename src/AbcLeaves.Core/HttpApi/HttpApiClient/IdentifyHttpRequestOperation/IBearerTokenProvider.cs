using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public interface IBearerTokenProvider
    {
        Task<AuthTokenResult> GetBearerToken();
    }
}
