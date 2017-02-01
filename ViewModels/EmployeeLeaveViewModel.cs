using System;
using ABC.Leaves.Api.Enums;

namespace ABC.Leaves.Api.ViewModels
{
    public class EmployeeLeaveViewModel
    {
        public EmployeeLeaveType Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string GoogleAuthToken { get; set; }
    }
}
