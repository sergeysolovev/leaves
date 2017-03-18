using System;
using System.Threading.Tasks;
using AbcLeaves.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AbcLeaves.Api.Repositories
{
    public class LeavesRepository: ILeavesRepository
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
