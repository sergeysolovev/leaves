using ABC.Leaves.Api.Models;

namespace ABC.Leaves.Api.Repositories
{
    public interface IEmployeeRepository
    {
        Employee Find(string gmailLogin);
    }
}
