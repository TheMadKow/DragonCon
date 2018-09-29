using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.RavenDB;
using DragonCon.RavenDB.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Identity;

namespace Dragoncon.App
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
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddLogging(opt =>
            {
                opt.SetMinimumLevel(LogLevel.Warning);
            });

            var holder = new StoreHolder("", ""); //TODO connection String

            services
                .AddRavenDbAsyncSession(holder.Store) // Create a RavenDB IAsyncDocumentSession for each request.
                .AddRavenDbIdentity<RavenSystemUser>(c => // Use Raven for users and roles. 
                {
                    c.User.RequireUniqueEmail = true;
                    c.Password.RequireDigit = true;
                    c.Password.RequireNonAlphanumeric = false;
                    c.Password.RequireUppercase = true;
                    c.Password.RequiredLength = 6;
                });

            services.ConfigureApplicationCookie(opt =>
            {
                // TODO: move to config
                opt.Cookie.Expiration = TimeSpan.FromDays(30);
                opt.LoginPath = "Users/Login";
                opt.LogoutPath = "Users/Logout";
                opt.AccessDeniedPath = "Users/Denied";
                opt.SlidingExpiration = true;
            });

            services.AddAntiforgery();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "admin",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // TODO verify database
        }
    }
}
