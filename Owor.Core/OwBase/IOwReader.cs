using Owor.Core.DeviceTypes;

namespace Owor.Core.OwBase
{

    internal interface IOwReader
    {

        IOwDevice[] GetRawDevices();

        IOwDevice GetRawDevice(string deviceId);

    }

}