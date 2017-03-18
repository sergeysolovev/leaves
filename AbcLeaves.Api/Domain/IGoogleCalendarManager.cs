using System.Threading.Tasks;

namespace AbcLeaves.Api.Domain
{
    public interface IGoogleCalendarManager
    {
        Task<OperationResult> PublishUserEventAsync(UserEventPublishDto userEvent);
    }
}
