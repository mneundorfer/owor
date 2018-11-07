using System;
using System.Collections.Generic;

namespace Owor.Shared
{
    public class OwDeviceDto
    {

        public string Id { get; set; }

        public string Description { get; set; }

        public KeyValuePair<string, object>[] MeasuredValues { get; set; }

        public DateTime LastRead { get; set; }

    }

}
