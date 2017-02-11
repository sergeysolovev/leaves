using System;
using System.ComponentModel.DataAnnotations;

namespace ABC.Leaves.Api.Leaves.Dto
{
    public class EmployeeLeaveDto
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Start { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime End { get; set; }

        [Required]
        public string GoogleAuthAccessToken { get; set; }
    }
}
