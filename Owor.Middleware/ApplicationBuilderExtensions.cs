using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Owor.Middleware.Diagnostics;
using Owor.Middleware.Performance;

namespace Owor.Middleware.BuilderExtensions
{

    public static class OworMiddelwareExtensions
    {

        public static IApplicationBuilder UseOworMiddleware(this IApplicationBuilder builder) 
        {
            builder.UseMiddleware<PerformanceCheckMiddleware>();
            builder.UseMiddleware<DiagnosticsMiddleware>();

            return builder;
        }

    }

}