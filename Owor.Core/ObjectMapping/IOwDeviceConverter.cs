using Owor.Core.DeviceTypes;
using Owor.Shared;

namespace Owor.Core.ObjectMapping
{

    internal interface IOwDeviceConverter
    {

        OwDeviceDto ToDto(IOwDevice device);

        OwDeviceDto[] ToDto(IOwDevice[] devices);
        
    }

}