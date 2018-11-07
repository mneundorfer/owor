using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owor.Core.FsAccess;
using Owor.Core.OwBase;

namespace Owor.Core.DeviceTypes
{

    /// <summary>
    /// EEPROM Device with internal flash storage (e.g. DS2433)
    /// </summary>
    internal sealed class EepromDevice : BasicOwDevice
    {
        public EepromDevice(ILoggerFactory loggerFactory, IOwFsReader fileReader) : base(loggerFactory, fileReader)
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
            var raw = _fileReader.GetFileContent(_deviceId, "eeprom");

            return raw;
        }
        
    }

}