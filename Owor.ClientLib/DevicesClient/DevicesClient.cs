using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Owor.Shared;

namespace Owor.ClientLib.Devices
{

    public class DevicesClient : IDevicesClient
    {

        private readonly ILogger<DevicesClient> _logger;
        private readonly HttpClient _httpClient;

        private readonly string[] DEVICES_BASE_PATH = { "api", "devices" };

        public DevicesClient(ILogger<DevicesClient> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;

            InitializeBaseAddress();
        }

        private void InitializeBaseAddress()
        {
            if (_httpClient.BaseAddress is null)
            {
                throw new ArgumentNullException("BaseAddress cannot be null!");
            }
            var fullUri = new Uri(_httpClient.BaseAddress, string.Join("/", DEVICES_BASE_PATH));
            _httpClient.BaseAddress = fullUri;
            _logger.LogInformation("Initializing devices client with base address {0}", _httpClient.BaseAddress);
        }

        public async Task<OwDeviceDto[]> GetDevicesAsync()
        {
            try
            {
                var raw = await _httpClient.GetStringAsync("");

                _logger.LogDebug("Retrieved {0} from {1}", raw, _httpClient.BaseAddress);

                return JsonConvert.DeserializeObject<OwDeviceDto[]>(raw);
            }
            catch (HttpRequestException)
            {
                _logger.LogError("Couldn't reach endpoint at {0}{1}", _httpClient.BaseAddress);
                throw;
            }
        }

        public async Task<OwDeviceDto> GetDeviceAsync(string deviceId)
        {
            try
            {
                var raw = await _httpClient.GetStringAsync(deviceId);

                _logger.LogDebug("Retrieved {0} from {1}", raw, _httpClient.BaseAddress);

                return JsonConvert.DeserializeObject<OwDeviceDto>(raw);
            }
            catch (HttpRequestException)
            {
                _logger.LogError("Couldn't reach endpoint at {0}{1}", _httpClient.BaseAddress);
                throw;
            }
        }

    }

}