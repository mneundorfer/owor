using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Owor.Core.FsAccess;

namespace Owor.Core.DeviceTypes
{

    internal abstract class BasicOwDevice : IOwDevice
    {
        
        protected readonly ILogger _logger;
        protected readonly IOwFsReader _fileReader;
        protected string _deviceId;

        public BasicOwDevice(ILoggerFactory loggerFactory, IOwFsReader fileReader)
        {
            _logger = loggerFactory.CreateLogger(this.GetType());
            _fileReader = fileReader;
        }

        public string DeviceId { get => _deviceId; set => _deviceId = value; }
        public string Description { get; set; }

        public abstract KeyValuePair<string, object>[] ReadDeviceValues();
        
    }

}