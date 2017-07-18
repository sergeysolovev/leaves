using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AbcLeaves.Api.Repositories;
using AbcLeaves.Api.Models;
using AbcLeaves.Api.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using Newtonsoft.Json;
using AbcLeaves.Api.Helpers;
using AbcLeaves.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AbcLeaves.Api
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

            if (Program.Args != null)
            {
                builder.AddCommandLine(Program.Args);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);

            services
                .AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            // Authorization:
            services.AddSingleton<IAuthorizationHandler, HasPersistentClaimAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, HasPersistentTokenAuthorizationHandler>();
            services.AddAuthorization(options => {
                options.AddPolicy("CanApplyLeaves", policyBuilder => policyBuilder
                    .AddRequirements(new HasPersistentTokenRequirement("Google", "refresh_token"))
                );
                options.AddPolicy("CanApproveLeaves", policyBuilder => policyBuilder
                    .AddRequirements(new HasPersistentClaimRequirement("ApproveLeaves", "Allowed"))
                );
                options.AddPolicy("CanDeclineLeaves", policyBuilder => policyBuilder
                    .AddRequirements(new HasPersistentClaimRequirement("DeclineLeaves", "Allowed"))
                );
            });

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.MigrationsAssembly("AbcLeaves.Api")));

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options => {
                options.ClaimsIdentity.UserIdClaimType = "email";
                options.ClaimsIdentity.UserNameClaimType = "email";
                options.Cookies.ApplicationCookie.AutomaticAuthenticate = false;
                options.Cookies.ApplicationCookie.AutomaticChallenge = true;
            });

            services.AddAutoMapper();

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddTransient<ModelStateHelper>();

            services.AddBackchannel();

            services.AddTransient<LeavesRepository>();
            services.AddTransient<UserManager>();
            services.AddTransient<LeavesManager>();
            services.AddTransient<GoogleCalendarManager>();

            services.AddOptions();
            services.Configure<GoogleOAuthOptions>(Configuration.GetSection("GoogleOAuth"));
            services.Configure<GoogleCalendarOptions>(Configuration.GetSection("GoogleCalendarApi"));
            services.AddTransient<GoogleOAuthClient>();
            services.AddTransient<GoogleCalendarClient>();

            // services.AddSwaggerGen(options =>
            // {
            //     options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
            //     {
            //         Title = "AbcLeaves API",
            //         Version = "v1",
            //     });
            // });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIdentity();

            app.UseJwtBearerAuthentication(new JwtBearerOptions {
                Authority = "https://accounts.google.com",
                Audience = Configuration["GoogleOAuth:ClientId"],
                TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true
                },
                BackchannelHttpHandler = app.ApplicationServices.GetHttpMessageHandler()
            });

            app.UseMvc();

            // app.UseSwagger();
            // app.UseSwaggerUi(options =>
            // {
            //     options.SwaggerEndpoint("/swagger/v1/swagger.json", "AbcLeaves API V1");
            // });

            // todo: move to extensions
            //SampleData.InitializeAppDatabaseAsync(app.ApplicationServices).Wait();
        }
    }
}
