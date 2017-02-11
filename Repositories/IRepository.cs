using System.Threading.Tasks;

namespace ABC.Leaves.Api.Repositories
{
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        TEntity GetById(TPrimaryKey id);
        void Insert(TEntity entity);
        Task InsertAsync(TEntity entity);
        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);
    }
}
