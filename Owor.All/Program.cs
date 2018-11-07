using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Owor.All
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .UseSerilog()
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) => {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configurationBuilder.Build())
                        .CreateLogger();
                })
                .UseStartup<Startup>();
    }
}
