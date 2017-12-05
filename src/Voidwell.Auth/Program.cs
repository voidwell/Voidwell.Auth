using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Voidwell.VoidwellAuth.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:5000")
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.Sources.Clear();
                    config.AddJsonFile("appsettings.json", true);
                    config.AddJsonFile("testsettings.json", true, true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging(builder =>
                {
                    //builder.AddFilter("Microsoft", LogLevel.Error);
                    builder.AddFilter("Microsoft.EntityFramework", LogLevel.Error);
                    builder.AddFilter("Microsoft.AspNetCore.Hosting", LogLevel.Error);
                    builder.AddFilter("Microsoft.AspNetCore.StaticFiles", LogLevel.Error);
                    builder.AddDebug();
                })
                .Build();
    }
}
