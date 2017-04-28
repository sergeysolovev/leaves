using System.Threading.Tasks;
using AbcLeaves.Api.Models;

namespace AbcLeaves.Api.Repositories
{
    public interface ILeavesRepository
    {
        Leave GetById(int id);
        Task InsertAsync(Leave entity);
        Task UpdateAsync(Leave entity);
    }
}
