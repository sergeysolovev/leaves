using System;

namespace AbcLeaves.Api
{
    public class LeaveApplyDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string UserId { get; set; }
    }
}
