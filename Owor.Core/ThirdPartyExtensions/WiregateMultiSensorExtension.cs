using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using Owor.Core.OwBase;
using Owor.Core.DeviceTypes;

namespace Owor.Core.ThirdPartyExtensions
{

    /// <summary>
    /// This extension is responsible for identifying Wiregate MultiSensors
    /// and adding them to the list of found devices
    /// 
    /// see also: https://shop.wiregate.de/sensoren-fuhler/multisensor/b-serie-standard.html
    /// "Auf den nicht mit LOX-/OEM gekennzeichneten Baugruppen befindet sich ein 1-Wire 
    /// Flash-Memory-Chip der vom WireGate / Timberwolf Server für benutzerfreundliches 
    /// Plug & Play verwendet wird. Dieser Chip bzw. dessen Speicherinhalt wird von keinem 
    /// der uns bekannten anderen System unterstützt / ausgelesen"
    /// 
    /// The EEPROM chip is read and temperature and humidity sensor are identified
    /// </summary>
    internal sealed class WiregateMultiSensorExtension : IThirdPartyExtension
    {
        
        private readonly ILogger<WiregateMultiSensorExtension> _logger;

        private readonly IOwDeviceFactory _deviceFactory;

        public WiregateMultiSensorExtension(ILogger<WiregateMultiSensorExtension> logger, IOwDeviceFactory deviceFactory)
        {
            _logger = logger;
            _deviceFactory = deviceFactory;
        }

        /// <summary>
        /// Figures out if an EEPROM chip (type 23) contains data which point towards a Wiregate MultiSensor
        /// and - if so - appends the appropriate device to the list of devices
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public IEnumerable<IOwDevice> Process(IEnumerable<IOwDevice> devices)
        {
            var eeproms = devices.Where(dev => dev is EepromDevice);
            var tmp = devices.ToList();

            _logger.LogInformation("Identified eeproms: {0}", eeproms.Select(ep => ep.DeviceId));

            foreach (var memChip in eeproms)
            {
                var eepromContent = memChip.ReadDeviceValues().Single().Value.ToString();
                
                string humSensorId = null, tempSensorId = null;
                try 
                {
                    (humSensorId, tempSensorId) = GetMultiSensorDeviceIds(eepromContent);

                    _logger.LogInformation("Extracted humidity sensor id {0} and temperature sensor id {1} for multisensor {2}", humSensorId, tempSensorId, memChip.DeviceId);
                }
                catch (ArgumentException ae)
                {
                    _logger.LogInformation("Eeprom {0} did not contain MultiSensor data - omitting it: {1}", memChip.DeviceId, ae.Message);
                }

                var wiregateMultiSensor = _deviceFactory.GetDevice($"WGMS-{string.Concat(humSensorId, tempSensorId)}") as MultiSensor;

                try
                {
                    var humSensor = devices.Single(dev => dev.DeviceId.Equals(humSensorId, StringComparison.InvariantCultureIgnoreCase));
                    wiregateMultiSensor.SetHumiditySensor(humSensor);

                    var tempSensor = devices.Single(dev => dev.DeviceId.Equals(tempSensorId, StringComparison.InvariantCultureIgnoreCase));
                    wiregateMultiSensor.SetTemperatureSensor(tempSensor);

                    tmp.Add(wiregateMultiSensor);
                }
                catch (InvalidOperationException)
                {
                    _logger.LogWarning("Eeprom {0} references humidity sensor id {1} and temperature sensor id {2}, but at least one of them could not be found!", memChip.DeviceId, humSensorId, tempSensorId);
                }
            }

            return tmp.AsEnumerable();
        }

        /// <summary>
        /// Identifies and returns the humidity sensor id and the temperature sensor
        /// id encoded in the passed hexString
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private (string, string) GetMultiSensorDeviceIds(string hexString)
        {
            try
            {
                var decoded = GetAsChar(hexString);

                _logger.LogDebug("Decoded {0} to {1]", hexString, decoded);

                var areas = decoded.Split(';');

                // part 7 is the id of the DS2438
                var encodedHumId = areas[7].Remove(0, 6);
                // part 8 is the id of the DS18B20
                var encodedTempId = areas[8].Remove(0, 6);

                var decodedHumId = GetFormattedId(encodedHumId);
                var decodedTempId = GetFormattedId(encodedTempId);

                return (decodedHumId, decodedTempId);
            }
            catch (Exception e) when (e is FormatException || e is IndexOutOfRangeException || e is ArgumentOutOfRangeException || e is ArgumentException)
            {
                throw new ArgumentException($"Invalid argument which could not be parsed as MultiSensor mem data: {hexString}: {e.Message}");
            }
        }

        /// <summary>
        /// Turns the Hex into its char representation. The result looks like
        /// 
        /// 001W-TH-30-B-S;02300;031519301340;04A;0581.799834000000;1390004870865;20000023.B8487C020000;20010126.38E127020000;20020428.4BD640090000;
        ///                                                                             |-------------|       |-------------|       |-------------|
        ///                                                                              EEPROM ID             HUM ID                TEMP ID
        /// 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private string GetAsChar(string hex)
        {
            // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/how-to-convert-between-hexadecimal-strings-and-numeric-types#example-1
            var hexValuesSplit = new List<string>();
            var hexAsChar = new List<string>();

            // Split the input into two-char segments
            for (var i = 0; i < hex.Length; i += 2)
                hexValuesSplit.Add(hex.Substring(i, Math.Min(2, hex.Length - i)));

                _logger.LogDebug("Split {0} into {@Split}", hex, hexValuesSplit);

            foreach (var hexSegment in hexValuesSplit)
            {
                // Convert the number expressed in base-16 to an integer.
                var value = Convert.ToInt32(hexSegment, 16);
                // Get the character corresponding to the integral value.
                var stringValue = Char.ConvertFromUtf32(value);
                hexAsChar.Add(stringValue);
            }

            return string.Join("", hexAsChar);
        }

        /// <summary>
        /// On the EERPOM, the ids of the sensors are "encoded" such that the values are swapped in two-char segments
        /// Also, the '.' separator is used instead of the '-' separator
        /// 
        /// Example
        /// -------
        /// Input:  26.38E127020000
        /// Output: 26-00000227E138
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetFormattedId(string input)
        {
            var parts = input.Split('.');
            return $"{parts[0]}-{SwapValues(parts[1])}";
        }

        private string SwapValues(string raw)
        {
            var blockSize = 2;

            if (raw.Length % blockSize != 0)
            {
                throw new ArgumentException($"Can only do a two-char block swap when the length of the string is even!");
            }

            var length = raw.Length;
            var swapped = new string[length / blockSize];
            for (var i = 0; i < length / blockSize; i++)
            {
                var startIndex = length - (blockSize + i) - i;
                swapped[i] = raw.Substring(length - (blockSize + i) - i, blockSize);
            }

            return string.Join("", swapped);
        }

    }

}