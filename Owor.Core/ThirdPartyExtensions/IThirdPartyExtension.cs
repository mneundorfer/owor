using System.Collections.Generic;
using Owor.Core.DeviceTypes;

namespace Owor.Core.ThirdPartyExtensions
{

    internal interface IThirdPartyExtension
    {

        IEnumerable<IOwDevice> Process(IEnumerable<IOwDevice> devices);

    }

}