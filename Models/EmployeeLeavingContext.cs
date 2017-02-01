using Microsoft.EntityFrameworkCore;

namespace ABC.Leaves.Api.Models
{
    public class EmployeeLeavingContext : DbContext
    {
        public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public EmployeeLeavingContext(DbContextOptions<EmployeeLeavingContext> options) :
            base(options)
        {
        }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlite("Filename=./EmployeeLeaving.db");
        // }
    }
}
