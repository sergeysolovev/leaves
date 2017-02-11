using ABC.Leaves.Api.Models;

namespace ABC.Leaves.Api.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee, string>
    {
        bool CheckUserIsAdmin(string email);
    }
}
