using System.Threading.Tasks;

namespace AbcLeaves.Api.Services
{
    public interface IGoogleCalendarService
    {
        Task<OperationResult> AddEventAsync(CalendarEventAddDto eventAddDto);
    }
}
