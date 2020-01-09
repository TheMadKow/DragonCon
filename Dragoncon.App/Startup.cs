using System;
using System.IO;
using DragonCon.App.Helpers;
using DragonCon.Features.Management;
using DragonCon.Features.Management.Convention;
using DragonCon.Features.Management.Dashboard;
using DragonCon.Features.Management.Events;
using DragonCon.Features.Management.Participants;
using DragonCon.Features.Management.Reception;
using DragonCon.Features.Shared;
using DragonCon.Logical;
using DragonCon.Logical.Communication;
using DragonCon.Logical.Convention;
using DragonCon.Logical.Factories;
using DragonCon.Logical.Gateways;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Identities.Policy;
using DragonCon.Modeling.TimeSlots;
using DragonCon.RavenDB;
using DragonCon.RavenDB.Gateways.Logic;
using DragonCon.RavenDB.Gateways.Management;
using DragonCon.RavenDB.Identities;
using DragonCon.RavenDB.Index;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Indexes;
using Raven.Client.NodaTime;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Raven.DependencyInjection;
using Raven.Identity;
using Serilog;

namespace DragonCon.App
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                //options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });


            services.AddAuthorization(options =>
            {
                foreach (var policy in Policies.GetPolicies())
                {
                    options.AddPolicy(policy.Key, p => p.Requirements.Add(policy.Value));
                }
            });


            services.AddAntiforgery();
            services.AddMvc(options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    options.EnableEndpointRouting = true;
                })
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson();

            services.AddHsts(opt =>
            {
                opt.Preload = true;
                opt.IncludeSubDomains = true;
            });
            services.AddHttpsRedirection(opt =>
            {
                opt.HttpsPort = 443;
                opt.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });

            services.AddLogging(opt =>
            {
                opt.SetMinimumLevel(LogLevel.Warning);
            });

            var database = "";
            if (_environment.EnvironmentName == "Development")
                database = StoreConsts.DatabaseName_Developement;
            
            if (_environment.EnvironmentName == "Staging")
                database = StoreConsts.DatabaseName_Staging;
            
            if (_environment.EnvironmentName == "Production")
                database = StoreConsts.DatabaseName_Production;
            
            var certificatePath = Path.Combine(_environment.ContentRootPath, StoreConsts.CertificatePath);
            var holder = new StoreHolder(database, certificatePath, StoreConsts.ConnectionString); 
            
            holder.Store.ConfigureForNodaTime();
            holder.Store.Initialize();
            if (holder.Store.Maintenance.Server.Send(new GetDatabaseRecordOperation(database)) == null)
            {
                var databaseRecord = new DatabaseRecord(database);
                holder.Store.Maintenance.Server.Send(new CreateDatabaseOperation(databaseRecord));
            };

            IndexCreation.CreateIndexes(typeof(EventsIndex_ByTitleDescription).Assembly, holder.Store);

            services.AddSingleton<StoreHolder>(holder);
            services // Create a RavenDB IAsyncDocumentSession for each request.
                .AddRavenDbAsyncSession(holder.Store)
                .AddRavenDbIdentity<LongTermParticipant>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 4;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                //options.Cookie.Expiration = TimeSpan.FromDays(30);
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.LoginPath = "/Users/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Users/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Users/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });

            services.AddScopedPolicyHandlers();
            StartupDependencyInjection(services);
            services.AddAntiforgery();
        }

        private static void StartupDependencyInjection(IServiceCollection services)
        {
            services.AddScoped<IActor, Actor>();
            services.AddSingleton<IStrategyFactory, StrategyFactory>();
            services.AddScoped<NullGateway, NullGateway>();
            services.AddScoped<IIdentityFacade, RavenIdentityFacade>();
            services.AddScoped<ICommunicationHub, CommunicationHub>();
            services.AddScoped<IManagementReceptionGateway, RavenManagementReceptionGateway>();
            services.AddScoped<IManagementConventionGateway, RavenManagementConventionGateway>();
            services.AddScoped<IManagementStatisticsGateway, RavenManagementStatisticsGateway>();
            services.AddScoped<IManagementEventsGateway, RavenManagementEventsGateway>();
            services.AddScoped<IManagementParticipantsGateway, RavenManagementParticipantsGateway>();
            services.AddScoped<IConventionBuilderGateway, RavenConventionBuilderGateway>();
            services.AddScoped<ConventionBuilder, ConventionBuilder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            var dir = env.WebRootPath;
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File($"{dir}\\Logs\\DragonCon_.txt",
                    Serilog.Events.LogEventLevel.Warning,
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();


            if (env.EnvironmentName == "Development")
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Convention/Error");
            }

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = res =>
                {
                    res.Context.Response.Headers.Append("Cache-Control", "public, max-age=2628000");
                }
            });
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseActorInitialization();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                        name: "General Management",
                        areaName:"Management",
                        pattern: "Management/{controller=Dashboard}/{action=Index}/{id?}")
                    .RequireAuthorization(Policies.Types.ManagementAreaViewer);

                endpoints.MapAreaControllerRoute(
                        name: "Participants",
                        areaName: "Users",
                        pattern: "Users/{controller=Home}/{action=Index}/{id?}")
                    .RequireAuthorization();

                endpoints.MapAreaControllerRoute(
                        name: "default",
                        areaName: "Convention",
                        pattern: "{area=Convention}/{controller=Home}/{action=Index}/{id?}");
            });

            // TODO verify database
        }
    }
}