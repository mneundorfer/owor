using System;

namespace Owor.Core.Exceptions
{

    public class DeviceTypeNotSupportedException : Exception
    {

        public DeviceTypeNotSupportedException(string message): base(message)
        {
            
        }

    }

}