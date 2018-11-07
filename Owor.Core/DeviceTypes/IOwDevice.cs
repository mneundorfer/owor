using System.Collections.Generic;

namespace Owor.Core.DeviceTypes
{

    internal interface IOwDevice
    {

        KeyValuePair<string, object>[] ReadDeviceValues();

        string DeviceId { get; set; }

        string Description { get; set; }

    }

}