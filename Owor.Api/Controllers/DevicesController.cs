using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Owor.Api.Cache;
using Owor.Core.Exceptions;
using Owor.Shared;

namespace Owor.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {

        private readonly ILogger<DevicesController> _logger;

        public IDeviceCache _deviceCache { get; }

        public DevicesController(ILogger<DevicesController> logger, IDeviceCache deviceCache)
        {
            _logger = logger;
            _deviceCache = deviceCache;
        }

        [HttpGet]
        public ActionResult<OwDeviceDto[]> Get()
        {
            _logger.LogDebug("Requesting all available devices");
            
            var devices = _deviceCache.GetDevices();

            return devices;
        }

        [HttpGet("{deviceId}")]
        public ActionResult<OwDeviceDto> Get(string deviceId)
        {
            _logger.LogDebug("Requesting device with id {0}", deviceId);

            try
            {
                var device = _deviceCache.GetDevice(deviceId);
                
                return Ok(device);
            }
            catch (InvalidOperationException)
            {
                return NotFound($"No device with Id {deviceId} has been found!");
            }
            catch (InvalidFileContentException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }

    }

}
