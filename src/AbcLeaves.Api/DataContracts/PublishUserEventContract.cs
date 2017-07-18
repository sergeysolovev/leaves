using System;

namespace AbcLeaves.Api
{
    public class PublishUserEventContract
    {
        public string UserId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
