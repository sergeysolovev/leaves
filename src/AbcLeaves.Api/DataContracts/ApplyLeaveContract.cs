using System;

namespace AbcLeaves.Api
{
    public class ApplyLeaveContract
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string UserId { get; set; }
    }
}
