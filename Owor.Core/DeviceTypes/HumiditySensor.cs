using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owor.Core.FsAccess;
using Owor.Core.OwBase;

namespace Owor.Core.DeviceTypes
{

    /// <summary>
    /// DS2438
    /// </summary>
    internal sealed class HumiditySensor : BasicOwDevice
    {

        public HumiditySensor(ILoggerFactory loggerFactory, IOwFsReader fileReader) : base(loggerFactory, fileReader)
        {

        }

        public override KeyValuePair<string, object>[] ReadDeviceValues()
        {
            return new[] {
                new KeyValuePair<string, object>(OwValue.HUMIDITY, GetHumidity())
            };
        }

        private double GetHumidity()
        {
            var vdd = GetVdd();
            var vad = GetVad();

            _logger.LogDebug($"Calculating humidity based on (vdd) {vdd} and (vad) {vad}");

            // https://shop.wiregate.de/download/Bedienungsanleitung_B_Serie_Standard_line.pdf (p. 18)
            var humidity = ((vad / vdd) - 0.16) / 0.0062;

            _logger.LogDebug($"Calculated absolute humidty of {humidity}");

            return humidity;
        }

        private double GetVdd()
        {
            var raw = _fileReader.GetFileContent(_deviceId, "vdd");

            var isDouble = double.TryParse(raw, out double vdd);

            if (!isDouble)
            {
                _logger.LogError("Wrong format!");
            }

            return vdd;
        }

        private double GetVad()
        {
            var raw = _fileReader.GetFileContent(_deviceId, "vad");

            var isDouble = double.TryParse(raw, out double vad);

            if (!isDouble)
            {
                _logger.LogError("Wrong format!");
            }

            return vad;
        }

    }

}