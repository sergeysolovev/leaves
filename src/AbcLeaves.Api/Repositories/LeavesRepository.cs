using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcLeaves.Api.Models;

namespace AbcLeaves.Api.Repositories
{
    public class LeavesRepository
    {
        private readonly AppDbContext dbContext;

        public LeavesRepository(AppDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            this.dbContext = dbContext;
        }

        public Leave GetById(int id)
        {
            var leave = dbContext.Find<Leave>(id);
            if (leave != null)
            {
                dbContext.Entry(leave)
                    .Reference(l => l.User)
                    .Load();
            }
            return leave;
        }

        public IAsyncEnumerable<Leave> GetByUserId(string userId)
        {
            return dbContext
                .Leaves
                .ToAsyncEnumerable()
                .Where(l => l.UserId == userId);
        }

        public IAsyncEnumerable<Leave> GetAll()
        {
            return dbContext
                .Leaves
                .ToAsyncEnumerable();
        }

        public async Task InsertAsync(Leave leave)
        {
            await dbContext.AddAsync(leave);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Leave leave)
        {
            leave.ConcurrencyStamp = Guid.NewGuid().ToString();
            dbContext.Update(leave);
            await dbContext.SaveChangesAsync();
        }
    }
}
