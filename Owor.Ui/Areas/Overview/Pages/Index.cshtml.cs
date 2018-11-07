using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Owor.ClientLib.Devices;
using Owor.Shared;
using Owor.Ui.Models;

namespace Owor.Ui.Pages
{
    public class IndexModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly IDevicesClient _devicesClient;

        public IList<OwDeviceIndexModel> Devices { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IDevicesClient devicesClient)
        {
            _logger = logger;
            _devicesClient = devicesClient;
        }

        public async Task OnGetAsync()
        {
            var devices = await _devicesClient.GetDevicesAsync();
            
            Devices = devices is null ? Array.Empty<OwDeviceIndexModel>() : ToIndexModel(devices);
        }

        private static OwDeviceIndexModel[] ToIndexModel(OwDeviceDto[] devices)
        {
            var tmp = new OwDeviceIndexModel[devices.Length];

            for (var i = 0; i< devices.Length; i++)
            {
                tmp[i] = new OwDeviceIndexModel {
                    Id = devices[i].Id,
                    NoValues = devices[i].MeasuredValues.Length,
                    Description = devices[i].Description,
                    LastRead = devices[i].LastRead
                };
            }

            return tmp.ToArray();
        }
    }

}
