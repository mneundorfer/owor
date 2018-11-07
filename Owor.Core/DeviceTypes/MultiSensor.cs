using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owor.Core.FsAccess;
using Owor.Core.OwBase;

namespace Owor.Core.DeviceTypes
{

    /// <summary>
    /// Multisensor (Wiregate) with humidity and temperature measurements (DS2438)
    /// https://shop.wiregate.de/sensoren-fuhler/multisensor/b-serie-standard.html
    /// </summary>
    internal sealed class MultiSensor : SpecialOwDevice
    {

        private IOwDevice _temperatureSensor;
        private IOwDevice _humiditySensor;

        public MultiSensor(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Description = "Wiregate MultiSensor";
        }

        public override KeyValuePair<string, object>[] ReadDeviceValues()
        {
            return new[] {
                new KeyValuePair<string, object>(OwValue.TEMPERATURE, GetTemperature()),
                new KeyValuePair<string, object>(OwValue.HUMIDITY, GetRelativeHumidity())
            };
        }

        public void SetTemperatureSensor(IOwDevice temperatureSensor)
        {
            _temperatureSensor = temperatureSensor as TemperatureSensor;
        }

        public void SetHumiditySensor(IOwDevice humiditySensor)
        {
            _humiditySensor = humiditySensor as HumiditySensor;
        }

        private double GetTemperature()
        {
            double.TryParse(_temperatureSensor.ReadDeviceValues().Single().Value.ToString(), out double temperature);

            return temperature;
        }

        private double GetAbsoluteHumidity()
        {
            double.TryParse(_humiditySensor.ReadDeviceValues().Single().Value.ToString(), out double humidity);

            return humidity;
        }

        private double GetRelativeHumidity()
        {
            var humidity = GetAbsoluteHumidity();
            var temp = GetTemperature();

            _logger.LogDebug($"Calculating humidity based on (humidity) {humidity} and (temp) {temp}");

            // https://shop.wiregate.de/download/Bedienungsanleitung_B_Serie_Standard_line.pdf (p. 18)
            var relativeHumidity = humidity / (1.0546 - (0.00216 * temp));

            _logger.LogDebug($"Calculated relative humidty of {relativeHumidity}");

            return relativeHumidity;
        }

    }

}