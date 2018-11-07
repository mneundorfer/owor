using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Owor.Core.DeviceTypes;
using Owor.Shared;

namespace Owor.Core.ObjectMapping
{

    internal sealed class OwDeviceConverter : IOwDeviceConverter
    {
        
        private readonly ILogger<OwDeviceConverter> _logger;

        public OwDeviceConverter(ILogger<OwDeviceConverter> logger)
        {
            _logger = logger;
        }
        
        public OwDeviceDto ToDto(IOwDevice device)
        {
            var deviceValues = Enumerable.Empty<KeyValuePair<string, object>>().ToArray();
            try
            {
                deviceValues = device.ReadDeviceValues();
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error when reading value of device {0}: {1}", device.DeviceId, e.Message);
            }

            var tmp = new OwDeviceDto
            {
                Id = device.DeviceId,
                Description = device.Description,
                MeasuredValues = deviceValues,
                LastRead = DateTime.Now
            };

            return tmp;
        }

        public OwDeviceDto[] ToDto(IOwDevice[] devices)
        {
            var tmp = new OwDeviceDto[devices.Length];

            for (var i = 0; i < devices.Length; i++)
            {
                tmp[i] = ToDto(devices[i]);
            }

            return tmp.ToArray();
        }

    }

}