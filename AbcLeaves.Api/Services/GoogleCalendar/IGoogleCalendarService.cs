using System;
using System.Threading.Tasks;

namespace ABC.Leaves.Api.Services
{
    public interface IGoogleCalendarService
    {
        Task<AddEventResult> AddEventAsync(string accessToken, DateTime start, DateTime end);
    }
}
