using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ABC.Leaves.Api.Repositories;
using ABC.Leaves.Api.Services;
using ABC.Leaves.Api.GoogleAuth;
using ABC.Leaves.Api.Models;
using ABC.Leaves.Api.Authorization;

namespace ABC.Leaves.Api
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<GoogleAuthOptions>(Configuration.GetSection("GoogleAuth"));
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc();
            services.AddDbContext<EmployeeLeavingContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddAutoMapper();
            services.AddTransient<IGoogleAuthService, GoogleAuthService>();
            services.AddTransient<IEmployeeLeavesService, EmployeeLeavesService>();
            services.AddTransient<IAuthorizationService, AuthorizationService>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddTransient<IEmployeeLeavesRepository, EmployeeLeavesRepository>();
            services.AddSingleton<IHttpClient, HttpClient>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseMvc();
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<EmployeeLeavingContext>().Database.Migrate();
            }
        }
    }
}
