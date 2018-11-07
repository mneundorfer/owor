using Owor.Core.DeviceTypes;

namespace Owor.Core.OwBase
{

    internal interface IOwDeviceFactory
    {

        IOwDevice GetDevice(string deviceType);

    }

}