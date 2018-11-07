using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Owor.ClientLib.Devices;
using Owor.Shared;
using Owor.Ui.Models;

namespace Owor.Ui.Pages
{
    public class DetailsModel : PageModel
    {

        private readonly ILogger<DetailsModel> _logger;
        private readonly IDevicesClient _devicesClient;

        public OwDeviceDetailsModel Device { get; set; }

        public DetailsModel(ILogger<DetailsModel> logger, IDevicesClient devicesClient)
        {
            _logger = logger;
            _devicesClient = devicesClient;
        }

        public async Task OnGetAsync(string deviceId)
        {
            var device = await _devicesClient.GetDeviceAsync(deviceId);

            Device = device is null ? new OwDeviceDetailsModel() : ToDetailsModel(device);
        }

        private static OwDeviceDetailsModel ToDetailsModel(OwDeviceDto device)
        {
            var measurements = new KeyValuePair<string, string>[device.MeasuredValues.Length];

            for (var i = 0; i < device.MeasuredValues.Length; i++) {
                measurements[i] = new KeyValuePair<string, string>(device.MeasuredValues[i].Key, device.MeasuredValues[i].Value.ToString());
            }
            
            var tmp = new OwDeviceDetailsModel
            {
                Id = device.Id,
                Description = device.Description,
                LastRead = device.LastRead,
                Values = measurements
            };

            return tmp;
        }

    }

}
