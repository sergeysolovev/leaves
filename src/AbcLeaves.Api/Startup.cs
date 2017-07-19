﻿using AutoMapper;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AbcLeaves.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();
            services.AddOptions();
            services.AddRouting(options => options.LowercaseUrls = true);

            // MVC:
            services
                .AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });

            // Identity:
            services
                .AddIdentity<AppUser, IdentityRole>(options => {
                    options.ClaimsIdentity.UserIdClaimType = "email";
                    options.ClaimsIdentity.UserNameClaimType = "email";
                })
                .AddUserManager<UserManager>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Authentication:
            // Goes after Identity to override DefaultAuthenticateScheme, DefaultChallengeScheme
            services
                .AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearerAuthentication(options => {
                    options.Authority = "https://accounts.google.com";
                    options.Audience = Configuration["GoogleOAuth:ClientId"];
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = true
                    };
                    options.BackchannelHttpHandler = services
                        .BuildServiceProvider()
                        .GetHttpMessageHandler();
                });
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // Authorization:
            services.AddTransient<IAuthorizationHandler, HasPersistentClaimAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, HasPersistentTokenAuthorizationHandler>();
            services.AddAuthorization(options => {
                options.AddPolicy("CanApplyLeaves", policyBuilder => policyBuilder
                    .AddRequirements(new HasPersistentTokenRequirement("Google", "refresh_token"))
                );
                options.AddPolicy("CanManageAllLeaves", policyBuilder => policyBuilder
                    .AddRequirements(new HasPersistentClaimRequirement("ManageAllLeaves", "Allowed"))
                );
                options.AddPolicy("CanApproveLeaves", policyBuilder => policyBuilder
                    .AddRequirements(new HasPersistentClaimRequirement("ApproveLeaves", "Allowed"))
                );
                options.AddPolicy("CanDeclineLeaves", policyBuilder => policyBuilder
                    .AddRequirements(new HasPersistentClaimRequirement("DeclineLeaves", "Allowed"))
                );
            });

            // EF:
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.MigrationsAssembly("AbcLeaves.Api")));

            // Clients:
            services.AddBackchannel();
            services.Configure<GoogleOAuthOptions>(Configuration.GetSection("GoogleOAuth"));
            services.Configure<GoogleCalendarOptions>(Configuration.GetSection("GoogleCalendarApi"));
            services.AddTransient<GoogleOAuthClient>();
            services.AddTransient<GoogleCalendarClient>();

            // Local:
            services.AddTransient<ModelStateHelper>();
            services.AddTransient<LeavesRepository>();
            services.AddTransient<UserManager>();
            services.AddTransient<LeavesManager>();
            services.AddTransient<GoogleCalendarManager>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
