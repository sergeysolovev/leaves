using System;
using ABC.Leaves.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ABC.Leaves.Api.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeLeavingContext context;

        public EmployeeRepository(IServiceProvider serviceProvider)
        {
            context = serviceProvider.GetService<EmployeeLeavingContext>();
        }

        public Employee Find(string gmailLogin)
        {
            return context.Find<Employee>(gmailLogin);
        }
    }
}
