using ABC.Leaves.Api.Models;

namespace ABC.Leaves.Api.Repositories
{
    public class EmployeeRepository : RepositoryBase<Employee, string>, IEmployeeRepository
    {
        public EmployeeRepository(EmployeeLeavingContext dbContext)
            : base(dbContext)
        {
        }

        // TODO: move to domain level
        public bool CheckUserIsAdmin(string email)
        {
            var user = GetById(email);
            return (user != null && user.IsAdmin);
        }
    }
}
