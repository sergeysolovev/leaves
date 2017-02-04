using System.ComponentModel.DataAnnotations;

namespace ABC.Leaves.Api.Models
{
    public class Employee
    {
        [Key]
        public string GmailLogin { get; set; }

        public bool IsAdmin { get; set; }

        public string GoogleCalendarId { get; set; }
    }
}
