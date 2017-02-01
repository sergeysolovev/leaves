using Microsoft.Extensions.DependencyInjection;
using ABC.Leaves.Api.Models;
using System;

namespace ABC.Leaves.Api.Repositories
{
    public class EmployeeLeavesRepository: IEmployeeLeavesRepository
    {
        private readonly IServiceProvider serviceProvider;

        public EmployeeLeavesRepository(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Insert(EmployeeLeave employeeLeave)
        {
            using (var context = serviceProvider.GetService<EmployeeLeavingContext>())
            {
                context.Add(employeeLeave);
                context.SaveChanges();
            }
        }

        public EmployeeLeave Get(string id)
        {
            using (var context = serviceProvider.GetService<EmployeeLeavingContext>())
            {
                return context.Find<EmployeeLeave>(id);
            }
        }

        public void Update(EmployeeLeave employeeLeave)
        {
            using (var context = serviceProvider.GetService<EmployeeLeavingContext>())
            {
                context.Update(employeeLeave);
                context.SaveChanges();
            }
        }
    }
}
