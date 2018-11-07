using System.IO.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Owor.Api.Cache;
using Owor.Api.Configuration;
using Owor.Core.BuilderExtensions;

namespace Owor.Api.BuilderExtensions
{

    public static class ApiServiceCollectionExtensions
    {

        public static IServiceCollection AddOworApi(this IServiceCollection services, IConfiguration configuration = null)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddOworCore(configuration);

            services.AddMemoryCache();

            services.AddSingleton<IDeviceCache, DeviceCache>();

            if (!(configuration is null))
            {
                services.AddOptions();
                services.Configure<CacheOptions>(options =>
                    configuration.GetSection("CacheOptions").Bind(options)
                );
            }

            return services;
        }

    }

}