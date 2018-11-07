using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Owor.Core.DeviceTypes;
using Owor.Core.Exceptions;
using Owor.Core.FsAccess;
using Owor.Core.ThirdPartyExtensions;

namespace Owor.Core.OwBase
{

    internal sealed class OwReader : IOwReader
    {
        private readonly ILogger<OwReader> _logger;
        private readonly IOwFsReader _fileReader;
        private readonly IOwDeviceFactory _deviceFactory;
        private readonly IEnumerable<IThirdPartyExtension> _tpExtensions;

        public OwReader(ILogger<OwReader> logger, IOwFsReader fileReader, IOwDeviceFactory deviceFactory, IEnumerable<IThirdPartyExtension> tpExtensions)
        {
            _logger = logger;
            _fileReader = fileReader;
            _deviceFactory = deviceFactory;
            _tpExtensions = tpExtensions;
        }

        public IOwDevice[] GetRawDevices()
        {
            _logger.LogDebug("Trying to retrieve all devices on the bus...");

            // First of all, identity all "default" devices on the bus
            var defaultDevices = GetDefaultDevices();

            // Now check for any "special" devices and replace them in the list
            var processedDevices = ProcessDevicesThroughTpExtensions(defaultDevices);

            return processedDevices.ToArray();
        }

        public IOwDevice GetRawDevice(string deviceId)
        {
            _logger.LogDebug("Trying to retrieve device {0}...", deviceId);

            var device = _deviceFactory.GetDevice(deviceId);

            // only in case the device is a basic device, we can directly return it
            // special devices types constructed by 3rd party extensions need special
            // treatment!
            if (!(device is BasicOwDevice)) {
                device = GetRawDevices().SingleOrDefault(d => d.DeviceId == deviceId);
            }
            
            return device;
        }

        private IEnumerable<IOwDevice> GetDefaultDevices()
        {
            var deviceIds = _fileReader.GetOwDeviceIds();

            _logger.LogDebug("Found the following ids on the bus: {0}", deviceIds);

            var defaultDevices = new List<IOwDevice>();
            foreach (var deviceId in deviceIds)
            {
                try
                {
                    defaultDevices.Add(_deviceFactory.GetDevice(deviceId));
                }
                catch (DeviceTypeNotSupportedException dte)
                {
                    _logger.LogWarning("Could not add device {0}: {1}", deviceId, dte.Message);
                }
            }

            _logger.LogInformation("Turned them into the following devices: {0}", defaultDevices);

            return defaultDevices;
        }

        private IEnumerable<IOwDevice> ProcessDevicesThroughTpExtensions(IEnumerable<IOwDevice> devices)
        {
            var processedDevices = devices;
            foreach (var tpExtension in _tpExtensions)
            {
                _logger.LogInformation("Processing devices through third party processor {0}", tpExtension.GetType().Name);
                try
                {
                    processedDevices = tpExtension.Process(devices);
                }
                catch (Exception e)
                {
                    _logger.LogWarning("Unhandled exception when processing through {0}: {1} at {2}", tpExtension.GetType().Name, e.Message, e.StackTrace);
                }
                _logger.LogInformation("Resulting device list is {0}", processedDevices);
            }

            return processedDevices;
        }

    }

}