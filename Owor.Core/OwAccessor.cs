using Microsoft.Extensions.Logging;
using Owor.Core.ObjectMapping;
using Owor.Core.OwBase;
using Owor.Shared;

namespace Owor.Core
{

    internal sealed class OwAccessor : IOwAccessor
    {
        
        private readonly ILogger<OwAccessor> _logger;
        private readonly IOwReader _owReader;
        private readonly IOwDeviceConverter _converter;

        public OwAccessor(ILogger<OwAccessor> logger, IOwReader owReader, IOwDeviceConverter converter)
        {
            _logger = logger;
            _owReader = owReader;
            _converter = converter;
        }

        public OwDeviceDto[] GetDevices()
        {
            return _converter.ToDto(_owReader.GetRawDevices());
        }

        public OwDeviceDto GetDevice(string deviceId)
        {
            return _converter.ToDto(_owReader.GetRawDevice(deviceId));
        }

    }

}