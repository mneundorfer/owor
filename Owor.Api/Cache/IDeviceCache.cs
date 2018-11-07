using Owor.Shared;

namespace Owor.Api.Cache
{

    public interface IDeviceCache
    {

        OwDeviceDto[] GetDevices();

        OwDeviceDto[] GetDevicesUncached();

        OwDeviceDto GetDevice(string deviceId);

        OwDeviceDto GetDeviceUncached(string deviceId);

    }

}