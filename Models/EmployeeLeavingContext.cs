using Microsoft.EntityFrameworkCore;

namespace ABC.Leaves.Api.Models
{
    public class EmployeeLeavingContext : DbContext
    {
        public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public EmployeeLeavingContext(DbContextOptions options) :
            base(options)
        {
        }
    }
}
