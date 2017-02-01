using ABC.Leaves.Api.Models;

namespace ABC.Leaves.Api.Repositories
{
    public interface IEmployeeLeavesRepository
    {
        void Insert(EmployeeLeave employeeLeave);
        EmployeeLeave Get(string oid);
        void Update(EmployeeLeave model);
    }
}
