using System;
using ABC.Leaves.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ABC.Leaves.Api.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IServiceProvider serviceProvider;

        public EmployeeRepository(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Employee Find(string gmailLogin)
        {
            using (var context = serviceProvider.GetService<EmployeeLeavingContext>())
            {
                return context.Find<Employee>(gmailLogin);
            }
        }
    }
}
