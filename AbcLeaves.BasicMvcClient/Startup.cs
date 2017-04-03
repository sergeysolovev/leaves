using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Threading.Tasks;
using AbcLeaves.Core.Helpers;
using AbcLeaves.BasicMvcClient.Domain;
using Newtonsoft.Json;
using AbcLeaves.Core;

namespace AbcLeaves.BasicMvcClient
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
            services
                .AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddOptions();
            services.Configure<GoogleOAuthOptions>(Configuration.GetSection("GoogleOAuth"));
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddTransient<IGoogleApisAuthManager, GoogleApisAuthManager>();
            services.AddTransient<IMvcActionResultHelper, MvcActionResultHelper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IAuthenticationManager, AuthenticationManager>();

            // todo: implement google calendar and oauth as api clients
            // and try to register 2 clients

            // todo: make Core.Abstractions assembly

            // todo: make Core.Web assembly with implementations

            // todo: change names of googleapis controller (client + server)

            services.AddBackchannel<HttpClientHandler>();
            services
                .AddHttpApiClient<LeavesApiClient, LeavesApiClientFactory>(
                    Configuration.GetSection("LeavesApi"))
                .AddBearerTokenIdentification<HttpContextBearerTokenProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Authentication:
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
                BackchannelHttpHandler = app.ApplicationServices.GetService<HttpMessageHandler>(),
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
        }
    }
}
