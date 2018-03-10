using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.Auth.Services;
using Voidwell.Auth.Data;
using IdentityServer4.Services;
using Newtonsoft.Json;
using System;
using Microsoft.AspNetCore.Http;
using Voidwell.Auth.Delegation;
using Voidwell.Auth.Clients;
using System.IdentityModel.Tokens.Jwt;
using Voidwell.Auth;
using Microsoft.AspNetCore.Authentication;

namespace Voidwell.VoidwellAuth.Client
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
            services.AddAntiforgery(options =>
            {
                options.Cookie = new CookieBuilder
                {
                    Name = "voidwell.xsrf",
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
            });

            services.ConfigureServiceProperties("voidwell.auth");

            services.AddEntityFrameworkContext(Configuration);

            services.AddAuthenticatedHttpClient();
            services.AddSingleton<IClaimsTransformation, ClaimsTransformer>();

            services.AddTransient<Func<ITokenCreationService>>(a => () => a.GetService<ITokenCreationService>());

            services.AddTransient<IDelegationTokenValidationService, DelegationTokenValidationService>();
            services.AddTransient<IDelegationGrantValidationService, DelegationGrantValidationService>();

            services.AddTransient<ICorsPolicyService, CorsPolicyService>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<Auth.Services.IAuthenticationService, Auth.Services.AuthenticationService>();
            services.AddTransient<IVoidwellClientStore, VoidwellClientStore>();
            services.AddTransient<IVoidwellResourceStore, VoidwellResourceStore>();
            services.AddSingleton<IUserManagementClient, UserManagementClient>();

            services.AddCors(options =>
            {
                options.AddPolicy("localdev", policy =>
                {
                    policy.WithOrigins("http://localdev.com", "http://auth.localdev.com")
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .AllowAnyMethod();
                });
            });

            services.AddIdentityServer(options =>
                {
                    options.IssuerUri = Configuration.GetValue<string>("Issuer");

                    options.Discovery.ShowIdentityScopes = false;
                    options.Discovery.ShowApiScopes = false;

                    options.InputLengthRestrictions.Scope = 800;

                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = false;
                })
                .AddDeveloperSigningCredential()
                .AddIdentityServerStores(Configuration)
                .AddProfileService<ProfileService>()
                .AddExtensionGrantValidator<DelegationGrantValidator>();

            services.AddAuthentication("voidwell")
                .AddCookie("voidwell", options =>
                {
                    options.SlidingExpiration = false;
                    options.ExpireTimeSpan = TimeSpan.FromHours(10);
                    options.Cookie = new CookieBuilder
                    {
                        Name = "voidwell",
                        SecurePolicy = CookieSecurePolicy.SameAsRequest
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.InitializeDatabases();
            app.SeedData();

            app.UseCors("localdev");

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseIdentityServer();

            app.UseMvc();
        }
    }
}
