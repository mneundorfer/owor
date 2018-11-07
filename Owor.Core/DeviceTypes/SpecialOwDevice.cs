using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Owor.Core.FsAccess;

namespace Owor.Core.DeviceTypes
{

    internal abstract class SpecialOwDevice : IOwDevice
    {
        
        protected readonly ILogger _logger;
        protected string _deviceId;

        public SpecialOwDevice(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(this.GetType());
        }

        public string DeviceId { get => _deviceId; set => _deviceId = value; }
        public string Description { get; set; }

        public abstract KeyValuePair<string, object>[] ReadDeviceValues();
        
    }

}