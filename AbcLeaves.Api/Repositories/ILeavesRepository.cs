using System.Threading.Tasks;
using ABC.Leaves.Api.Models;

namespace ABC.Leaves.Api.Repositories
{
    public interface ILeavesRepository
    {
        Leave GetById(int id);
        Task InsertAsync(Leave entity);
        Task UpdateAsync(Leave entity);
    }
}
