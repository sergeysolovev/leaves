using System.Threading.Tasks;
using AbcLeaves.Core;

namespace AbcLeaves.Api.Domain
{
    public interface IGoogleCalendarManager
    {
        Task<OperationResult> PublishUserEventAsync(UserEventPublishDto userEvent);
    }
}
