using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Owor.Middleware.Diagnostics
{

    public class DiagnosticsMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<DiagnosticsMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public DiagnosticsMiddleware(RequestDelegate next, ILogger<DiagnosticsMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;
            if (path.Equals(PathString.FromUriComponent("/ping")))
            {
                await PingEndpoint(context);
            }
            else if (path.Equals(PathString.FromUriComponent("/config")))
            {
                await ConfigurationEndpoint(context);
            }
            else
            {
                await _next(context);
            }
        }

        private async Task PingEndpoint(HttpContext context)
        {
            _logger.LogInformation("Hitting 'ping' endpoint");

            context.Response.StatusCode = 204;
            await Task.CompletedTask;
        }

        private async Task ConfigurationEndpoint(HttpContext context)
        {
            _logger.LogInformation("Hitting 'configuration' endpoint");

            var configItems = new Dictionary<string, string>();

            foreach (var item in _configuration.AsEnumerable() )
            {
                var key = item.Key;
                var val = item.Value;
                configItems.Add(key, val);
            }

            await context.Response.WriteAsync(JsonConvert.SerializeObject(configItems, Formatting.Indented));
        }

    }

}