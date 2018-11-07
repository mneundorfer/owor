using Owor.Shared;

namespace Owor.Core
{

    public interface IOwAccessor
    {

        OwDeviceDto[] GetDevices();

        OwDeviceDto GetDevice(string deviceId);

    }

}