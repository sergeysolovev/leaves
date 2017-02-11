using ABC.Leaves.Api.Models;

namespace ABC.Leaves.Api.Repositories
{
    public class EmployeeLeavesRepository: RepositoryBase<EmployeeLeave, int>, IEmployeeLeavesRepository
    {
        public EmployeeLeavesRepository(EmployeeLeavingContext dbContext)
            : base(dbContext)
        {
        }
    }
}
