using System;
using System.Threading.Tasks;
using ABC.Leaves.Api.GoogleCalendar.Dto;

namespace ABC.Leaves.Api.GoogleAuth
{
    public interface IGoogleCalendarService
    {
        Task<AddEventResult> AddEventAsync(string accessToken, DateTime start, DateTime end);
    }
}
