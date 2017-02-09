using ABC.Leaves.Api.Models;

namespace ABC.Leaves.Api.Repositories
{
    public interface IEmployeeLeavesRepository
    {
        void Insert(EmployeeLeave employeeLeave);
        EmployeeLeave Find(int id);
        void Update(EmployeeLeave model);
    }
}
