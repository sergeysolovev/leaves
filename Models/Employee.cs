using ABC.Leaves.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace ABC.Leaves.Api.Models
{
    public class Employee
    {
        public EmployeeRights Rights { get; set; }

        [Key]
        public string GmailLogin { get; set; }
    }
}
