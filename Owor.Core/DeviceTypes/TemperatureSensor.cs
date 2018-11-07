using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owor.Core.Exceptions;
using Owor.Core.FsAccess;
using Owor.Core.OwBase;

namespace Owor.Core.DeviceTypes
{

    /// <summary>
    /// Temperature sensor (e.g. DS18B20(+), DS18S20)
    /// </summary>
    internal sealed class TemperatureSensor : BasicOwDevice
    {
        public TemperatureSensor(ILoggerFactory loggerFactory, IOwFsReader fileReader) : base(loggerFactory, fileReader)
        {
            Description = "Temperature sensor";
        }

        public override KeyValuePair<string, object>[] ReadDeviceValues()
        {
            return new[] {
                new KeyValuePair<string, object>(OwValue.TEMPERATURE, GetTemperature())
            };
        }

        public double GetTemperature()
        {
            var raw = _fileReader.GetFileContent(_deviceId, "w1_slave");

            var tempString = ExtractStringTemperature(raw);

            if (double.TryParse(tempString, out double temp))
            {
                return temp / 1000.0;
            }

            throw new InvalidFileContentException($"Could not extract temperature from file content: {raw}");
        }

        private string ExtractStringTemperature(string raw)
        {
            try
            {
                var secondLine = raw.Split('\n')[1];
                var rawTemp = secondLine.Split(' ')[9];

                _logger.LogDebug($"Extracted temp segment {rawTemp} from {raw}");

                return rawTemp.Split('=')[1];
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidFileContentException($"Could not extract temperature from file content: {raw}");
            }
        }

    }

}