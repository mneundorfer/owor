using System.IO.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Owor.Core.Configuration;
using Owor.Core.DeviceTypes;
using Owor.Core.FsAccess;
using Owor.Core.ObjectMapping;
using Owor.Core.OwBase;
using Owor.Core.ThirdPartyExtensions;

namespace Owor.Core.BuilderExtensions
{

    public static class CoreServiceCollectionExtensions
    {

        public static IServiceCollection AddOworCore(this IServiceCollection services, IConfiguration configuration = null)
        {
            services.AddTransient<IOwFsReader, OwFsReader>();

            services.AddTransient<IFileSystem, FileSystem>();

            services.AddTransient<IOwDevice, EepromDevice>();
            services.AddTransient<IOwDevice, HumiditySensor>();
            services.AddTransient<IOwDevice, MultiSensor>();
            services.AddTransient<IOwDevice, TemperatureSensor>();
            services.AddTransient<IOwDevice, IoDevice>();

            services.AddTransient<IOwReader, OwReader>();
            services.AddTransient<IOwDeviceFactory, OwDeviceFactory>();

            services.AddTransient<IThirdPartyExtension, WiregateMultiSensorExtension>();

            services.AddTransient<IOwDeviceConverter, OwDeviceConverter>();

            services.AddTransient<IOwAccessor, OwAccessor>();

            if (!(configuration is null))
            {
                services.AddOptions();
                services.Configure<OwConfig>(options =>
                    configuration.GetSection("OwConfig").Bind(options)
                );
            }

            return services;
        }

    }

}