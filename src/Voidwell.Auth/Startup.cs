using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.Auth;
using Voidwell.Auth.Filters;
using Voidwell.Auth.Services;
using Voidwell.Auth.Data;
using IdentityServer4.Services;
using Newtonsoft.Json;

namespace Voidwell.VoidwellAuth.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddDataAnnotations()
                .AddMvcOptions(options =>
                {
                    options.Filters.AddService(typeof(InvalidSecurityQuestionFilter));
                });

            services.AddTransient<ICorsPolicyService, CorsPolicyService>();
            services.AddTransient<IRegistrationService, RegistrationService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<ISecurityQuestionService, SecurityQuestionService>();
            services.AddSingleton<IUserCryptography, UserCryptography>();

            services.AddTransient<InvalidSecurityQuestionFilter>();

            services.AddVoidwellIdentityServer(Configuration);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseVoidwellIdentityServer();

            app.SeedData();

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
