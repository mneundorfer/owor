using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Owor.Middleware.Performance
{

    public class PerformanceCheckMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceCheckMiddleware> _logger;

        public PerformanceCheckMiddleware(RequestDelegate next, ILogger<PerformanceCheckMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = new Stopwatch();
            sw.Start();

            await _next.Invoke(context);

            sw.Stop();

            var isAssetsRequest = context.Request.Path.Value.EndsWith(".js") || context.Request.Path.Value.EndsWith(".css");
            var logMessage = $"Request to {context.Request.Path} took a total of {sw.ElapsedMilliseconds}ms";
            if (sw.ElapsedMilliseconds > 750)
            {
                _logger.LogWarning(logMessage);
            }
            else if (!isAssetsRequest)
            {
                _logger.LogDebug(logMessage);
            }
        }

    }

}