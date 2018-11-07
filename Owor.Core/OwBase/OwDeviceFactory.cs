using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Owor.Core.DeviceTypes;
using Owor.Core.Exceptions;

namespace Owor.Core.OwBase
{

    internal sealed class OwDeviceFactory : IOwDeviceFactory
    {

        private readonly ILogger<OwDeviceFactory> _logger;

        private readonly IServiceProvider _serviceProvider;

        public OwDeviceFactory(ILogger<OwDeviceFactory> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        private readonly Dictionary<string, Type> _supportedDeviceTypes = new Dictionary<string, Type> {
            { "3A", typeof(IoDevice) },
            { "10", typeof(TemperatureSensor) },
            { "23", typeof(EepromDevice) },
            { "26", typeof(HumiditySensor) },
            { "28", typeof(TemperatureSensor) },
            { "WGMS", typeof(MultiSensor) }
        };

        public IOwDevice GetDevice(string deviceId)
        {
            var deviceType = deviceId.Split('-')[0];

            _logger.LogDebug($"Trying to find device for deviceId {deviceId}.");
            
            if (!_supportedDeviceTypes.ContainsKey(deviceType))
            {
                throw new DeviceTypeNotSupportedException($"Device type {deviceType} is not supported and will be omitted!");
            }

            var device = _serviceProvider.GetServices<IOwDevice>().Single(d => d.GetType() == _supportedDeviceTypes[deviceType]);

            if (device is null)
            {
                throw new DeviceTypeNotSupportedException($"Device type {deviceType} should be supported, but has not been registered!");
            }

            device.DeviceId = deviceId;

            _logger.LogDebug($"Returning device {device.GetType().Name} based on device type {deviceType}");

            return device;
        }

    }

}