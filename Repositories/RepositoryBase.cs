using System.Threading.Tasks;
using ABC.Leaves.Api.Models;

namespace ABC.Leaves.Api.Repositories
{
    public class RepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class
    {
        protected readonly EmployeeLeavingContext dbContext;

        public RepositoryBase(EmployeeLeavingContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public TEntity GetById(TPrimaryKey id)
        {
            return dbContext.Find<TEntity>(id);
        }

        public void Insert(TEntity entity)
        {
            dbContext.Add(entity);
            dbContext.SaveChanges();
        }

        public async Task InsertAsync(TEntity entity)
        {
            await dbContext.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            dbContext.Update(entity);
            dbContext.SaveChanges();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            dbContext.Update(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
