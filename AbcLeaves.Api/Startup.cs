using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ABC.Leaves.Api.Repositories;
using ABC.Leaves.Api.Models;
using ABC.Leaves.Api.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json;
using ABC.Leaves.Api.Helpers;
using ABC.Leaves.Api.Domain;
using Microsoft.AspNetCore.Authorization;

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
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddCommandLine(Program.CommandLineArgs);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // server:
            services.Configure<Services.GoogleOAuthOptions>(Configuration.GetSection("GoogleServices:Auth"));
            services.Configure<GoogleCalendarOptions>(Configuration.GetSection("GoogleServices:Calendar"));

            // client:
            services.Configure<Helpers.GoogleOAuthOptions>(Configuration.GetSection("GoogleOAuth"));

            services.AddRouting(options => options.LowercaseUrls = true);

            services
                .AddMvc()
                // server:
                .AddJsonOptions(options => {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            // server:
            services.AddAuthorization(options => {
                options.AddPolicy("CanApproveLeaves",
                    policyBuilder => policyBuilder.AddRequirements(
                        new HasPersistentClaimRequirement("ApproveLeaves", "Allowed")));
                options.AddPolicy("CanDeclineLeaves",
                    policyBuilder => policyBuilder.AddRequirements(
                        new HasPersistentClaimRequirement("DeclineLeaves", "Allowed")));
            });

            // server:
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"),
                optionsBuilder => optionsBuilder.MigrationsAssembly("AbcLeaves")));
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options => {
                options.Cookies.ApplicationCookie.AutomaticAuthenticate = false;
                options.Cookies.ApplicationCookie.AutomaticChallenge = true;
            });

            services.AddSingleton<IAuthorizationHandler, HasPersistentClaimAuthorizationHandler>();

            services.AddSingleton<HttpClientHandler>();

            services.AddSingleton<IConfiguration>(Configuration);

            // client:
            services.AddTransient<IGoogleOAuthHelper, GoogleOAuthHelper>();

            // server:
            services.AddAutoMapper();

            services.AddTransient<IModelStateHelper, ModelStateHelper>();
            services.AddTransient<IGoogleOAuthService, GoogleOAuthService>();
            services.AddTransient<IGoogleCalendarService, GoogleCalendarService>();
            services.AddTransient<ILeavesRepository, LeavesRepository>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<ILeavesManager, LeavesManager>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "AbcLeaves API",
                    Version = "v1",
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // server:
            app.UseIdentity();
            app.UseJwtBearerAuthentication(new JwtBearerOptions {
                Authority = "https://accounts.google.com",
                Audience = Configuration["GoogleServices:Auth:ClientId"],
                TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true
                },
                BackchannelHttpHandler = app.ApplicationServices.GetService<HttpClientHandler>()
            });

            // client:
            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationScheme = "GoogleOpenIdConnectCookies",
                AutomaticAuthenticate = false,
                AutomaticChallenge = false
            });
            var openIdConnectOptions = new OpenIdConnectOptions() {
                AuthenticationScheme = "GoogleOpenIdConnect",
                SignInScheme = "GoogleOpenIdConnectCookies",
                Authority = "https://accounts.google.com",
                ResponseType = OpenIdConnectResponseType.IdToken,
                CallbackPath = new PathString("/signin-oidc"),
                ClientId = Configuration["GoogleOAuth:ClientId"],
                ClientSecret = Configuration["GoogleOAuth:ClientSecret"],
                AutomaticAuthenticate = false,
                AutomaticChallenge = false,
                GetClaimsFromUserInfoEndpoint = false,
                SaveTokens = true,
                UseTokenLifetime = true,
                BackchannelHttpHandler = app.ApplicationServices.GetService<HttpClientHandler>(),
                Events = new OpenIdConnectEvents()
                {
                    OnTicketReceived = context => {
                        context.Properties.IsPersistent = true;
                        context.Properties.AllowRefresh = false;
                        return Task.CompletedTask;
                    }
                }
            };
            openIdConnectOptions.Scope.Clear();
            openIdConnectOptions.Scope.Add("openid");
            openIdConnectOptions.Scope.Add("email");
            app.UseOpenIdConnectAuthentication(openIdConnectOptions);

            app.UseMvc();

            SampleData.InitializeAppDatabaseAsync(app.ApplicationServices).Wait();

            app.UseSwagger();
            app.UseSwaggerUi(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "AbcLeaves API V1");
            });
        }
    }
}
