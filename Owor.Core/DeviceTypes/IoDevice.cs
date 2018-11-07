using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owor.Core.FsAccess;
using Owor.Core.OwBase;

namespace Owor.Core.DeviceTypes
{

    /// <summary>
    /// I/O Device (e.g. DS2413)
    /// </summary>
    internal sealed class IoDevice : BasicOwDevice
    {
        public IoDevice(ILoggerFactory loggerFactory, IOwFsReader fileReader) : base(loggerFactory, fileReader)
        {

        }

        public override KeyValuePair<string, object>[] ReadDeviceValues()
        {
            return new[] {
                new KeyValuePair<string, object>(OwValue.RAW, GetEeprom())
            };
        }

        public string GetEeprom()
        {
            var raw = _fileReader.GetFileContent(_deviceId, "sensed.A");

            return raw;
        }
        
    }

}