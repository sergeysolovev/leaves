using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AbcLeaves.Api.Models
{
    public static class SampleData
    {
        public static async Task InitializeAppDatabaseAsync(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider
                .GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
                await CreateAdminUsersAsync(serviceProvider);
                // if (await dbContext.Database.EnsureCreatedAsync())
                // {
                // }
            }
        }

        private static async Task CreateAdminUsersAsync(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            var userManager = serviceProvider.GetService<UserManager<AppUser>>();

            var admins = configuration["Admins"];
            if (!String.IsNullOrEmpty(admins))
            {
                var adminsEmails = admins.Split(',')
                    .Select(x => x.Trim())
                    .ToArray();
                foreach (var adminEmail in adminsEmails)
                {
                    var user = await userManager.FindByEmailAsync(adminEmail);
                    if (user == null)
                    {
                        user = new AppUser {
                            Id = adminEmail,
                            UserName = adminEmail
                        };
                        await userManager.CreateAsync(user);
                        await userManager.AddClaimAsync(user, new Claim("ApproveLeaves", "Allowed"));
                        await userManager.AddClaimAsync(user, new Claim("DeclineLeaves", "Allowed"));
                    }
                }
            }
        }
    }
}

