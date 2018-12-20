using System;
using DragonCon.Features.Management.Convention;
using DragonCon.Features.Shared;
using DragonCon.Logical.Convention;
using DragonCon.Logical.Gateways;
using DragonCon.RavenDB;
using DragonCon.RavenDB.Gateways.Logic;
using DragonCon.RavenDB.Gateways.Management;
using DragonCon.RavenDB.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raven.Client.NodaTime;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Raven.Identity;

namespace DragonCon.App
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

            var holder = new StoreHolder(DragonConsts.DatabaseName, "http://127.0.0.1:8080"); //TODO connection String
            holder.Store.ConfigureForNodaTime();
            holder.Store.Initialize();
            if (holder.Store.Maintenance.Server.Send(new GetDatabaseRecordOperation(DragonConsts.DatabaseName)) == null)
            {
                var databaseRecord = new DatabaseRecord(DragonConsts.DatabaseName);
                holder.Store.Maintenance.Server.Send(new CreateDatabaseOperation(databaseRecord));
            };

            services.AddSingleton<StoreHolder>(holder);
            services.AddScoped<NullGateway, NullGateway>();
            services.AddScoped<IConventionGateway, RavenConventionGateway>();
            services.AddScoped<IConventionBuilderGateway, RavenConventionBuilderGateway>();
            services.AddScoped<ConventionBuilder, ConventionBuilder>();

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
                opt.LoginPath = "/Users/Login";
                opt.LogoutPath = "/Users/Logout";
                opt.AccessDeniedPath = "/Users/Denied";
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
                    template: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // TODO verify database
        }
    }
}
