using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AbcLeaves.Api.Models
{
    public class AppUser : IdentityUser
    {
        public List<Leave> Leaves { get; set; }
    }
}
