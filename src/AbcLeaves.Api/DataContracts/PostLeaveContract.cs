using System;
using System.ComponentModel.DataAnnotations;

namespace AbcLeaves.Api
{
    public class PostLeaveContract
    {
        [Required, UtcDate]
        public DateTime? Start { get; set; }

        [Required, UtcDate]
        public DateTime? End { get; set; }
    }
}
