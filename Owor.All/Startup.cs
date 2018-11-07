using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Owor.Api.BuilderExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Owor.Api.Controllers;
using Owor.Middleware.BuilderExtensions;
using Owor.ClientLib.Devices;
using System;

namespace Owor.All
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogDebug("Configuring services...");

            services.AddOworApi(Configuration);
            services.AddHttpClient<IDevicesClient, DevicesClient>(client =>
            {
                client.BaseAddress = new Uri(Configuration["OworApiEndpoint"]);
                client.Timeout = TimeSpan.FromMinutes(1);
            });

            services.AddMvc()
                // Add all app parts from the Owor.Api package
                .AddApplicationPart(typeof(DevicesController).Assembly)
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/Overview/Index", "");
                });
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            _logger.LogDebug("Setting up request pipeline...");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOworMiddleware();

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }

    }
}
