using System;

namespace Leaves.Api
{
    public class AddCalendarEventContract
    {
        public string AccessToken { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
