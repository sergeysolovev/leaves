using System;
using System.ComponentModel.DataAnnotations;

namespace AbcLeaves.Api
{
    public class LeavePostDto
    {
        [Required, UtcDate]
        public DateTime? Start { get; set; }

        [Required, UtcDate]
        public DateTime? End { get; set; }
    }
}
