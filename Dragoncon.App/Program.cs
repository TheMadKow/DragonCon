using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DragonCon.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var dir = Environment.CurrentDirectory;
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File($"{dir}\\Logs\\DragonCon_.txt", Serilog.Events.LogEventLevel.Warning,
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Warning("Starting up");
                BuildWebHost(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder BuildWebHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel()
                        .ConfigureKestrel(serverOptions =>
                        {
                            // Set properties and call methods on options
                        })
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseIISIntegration()
                        .UseSerilog()
                        .UseStartup<Startup>();
                });
    }
}