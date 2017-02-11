using ABC.Leaves.Api.Services.Dto;

namespace ABC.Leaves.Api.GoogleCalendar.Dto
{
    public class AddEventResult
    {
        public bool EventAdded { get; set; }
        public string EventUri { get; set; }
        public ErrorDto Error { get; set; }
    }
}
