using System;
using System.Collections.Generic;

namespace Owor.Ui.Models
{

    public class OwDeviceDetailsModel
    {

        public string Id { get; set; }

        public string Description { get; set; }

        public DateTime LastRead { get; set; }

        public KeyValuePair<string, string>[] Values { get; set; }

    }

}