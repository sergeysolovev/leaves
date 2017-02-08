using System;

namespace ABC.Leaves.Api.Dto
{
    public class EmployeeLeaveDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string GoogleAuthAccessToken { get; set; }
    }
}
