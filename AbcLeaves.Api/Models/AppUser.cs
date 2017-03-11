using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ABC.Leaves.Api.Models
{
    public class AppUser : IdentityUser
    {
        public List<Leave> Leaves { get; set; }
    }
}
