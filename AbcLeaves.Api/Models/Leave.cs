using System;

namespace ABC.Leaves.Api.Models
{
    public class Leave
    {
        public int Id { get; set; }
        public LeaveStatus Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}
