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
using Newtonsoft.Json;
using Leaves.Client.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Leaves.Client
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
            services.AddOptions();
            services.AddRouting(options => options.LowercaseUrls = true);

            // mvc:
            services
                .AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            // authentication:
            services
                .AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookieAuthentication()
                .AddOpenIdConnectAuthentication(options => {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "https://accounts.google.com";
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    options.CallbackPath = new PathString("/signin-oidc");
                    options.ClientId = Configuration["GoogleOAuth:ClientId"];
                    options.ClientSecret = Configuration["GoogleOAuth:ClientSecret"];
                    options.GetClaimsFromUserInfoEndpoint = false;
                    options.SaveTokens = true;
                    options.UseTokenLifetime = true;

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("email");

                    options.Events = new OpenIdConnectEvents() {
                        OnTicketReceived = context => {
                            context.Properties.IsPersistent = true;
                            context.Properties.AllowRefresh = false;
                            return Task.CompletedTask;
                        }
                    };

                    options.BackchannelHttpHandler = services
                        .BuildServiceProvider()
                        .GetHttpMessageHandler();
                });

            // auth helpers:
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<AuthHelper>();

            // google oauth:
            services.Configure<GoogleOAuthOptions>(Configuration.GetSection("GoogleOAuth"));
            services.AddTransient<GoogleOAuthHelper>();

            // api client:
            services.Configure<ApiOptions>(Configuration.GetSection("LeavesApi"));
            services.AddTransient<ApiClient>();

            // backchannel:
            services.AddBackchannel();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsProduction())
            {
                // Otherwise, redirects from google oidc
                // would lead to http-endpoints that causes redirects
                // from http -> https, POST -> GET
                app.Use((context, next) =>
                {
                    context.Request.Scheme = "https";
                    return next();
                });
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
