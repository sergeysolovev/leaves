using ABC.Leaves.Api.Models;

namespace ABC.Leaves.Api.Repositories
{
    public interface IEmployeeLeavesRepository
    {
        void Insert(EmployeeLeave employeeLeave);
        EmployeeLeave Find(string id);
        void Update(EmployeeLeave model);
    }
}
