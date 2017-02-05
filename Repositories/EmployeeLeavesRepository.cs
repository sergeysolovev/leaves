using Microsoft.Extensions.DependencyInjection;
using ABC.Leaves.Api.Models;
using System;

namespace ABC.Leaves.Api.Repositories
{
    public class EmployeeLeavesRepository: IEmployeeLeavesRepository
    {
        private readonly EmployeeLeavingContext context;

        public EmployeeLeavesRepository(IServiceProvider serviceProvider)
        {
            context = serviceProvider.GetService<EmployeeLeavingContext>();
        }

        public void Insert(EmployeeLeave employeeLeave)
        {
            context.Add(employeeLeave);
            context.SaveChanges();
        }

        public EmployeeLeave Find(string id)
        {
            return context.Find<EmployeeLeave>(id);
        }

        public void Update(EmployeeLeave employeeLeave)
        {
            context.Update(employeeLeave);
            context.SaveChanges();
        }
    }
}
