using System;
using ABC.Leaves.Api.Enums;

namespace ABC.Leaves.Api.Models
{
    public class EmployeeLeave
    {
        public string Id { get; set; }
        public EmployeeLeaveStatus Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string GoogleAuthAccessToken { get; set; }
    }
}
