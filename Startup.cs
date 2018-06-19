using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using bunqAggregation.Core;
using bunqAggregation.Intergration.bunq;

namespace bunqAggregation
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            if (!(File.Exists(@"bunq.conf")))
            {
                Connection.Register(env);
            }
            else
            {
                Connection.Initialize();
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters()
                .AddRazorViewEngine();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
                AutomaticChallenge = false,
                ExpireTimeSpan = System.TimeSpan.FromMinutes(15)
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "oidc",
                SignInScheme = "Cookies",

                Authority = Config.Authority,
                RequireHttpsMetadata = false,

                ClientId = "bunqaggregation",
                ClientSecret = Config.Secret,

                ResponseType = "code id_token",
                Scope = { "bunqaggregation" },

                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true,
                AutomaticChallenge = false
            });

            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                AuthenticationScheme = "Bearer",
                Authority = Config.Authority,
                RequireHttpsMetadata = false,
                ApiName = "bunqaggregation",
                AutomaticChallenge = false
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/Error", "?status={0}");
            app.UseMvcWithDefaultRoute();
        }
    }
}