using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMisty
{
    // CLASS FROM EXISTING SYSTEM TO ADD DATA TO EXISTING COSMOS DATABASE...

    public class WSPayload
    {
        public string id { get; set; }

        public int ProjectID { get; set; }

        public int RoleID { get; set; }

        public string DeviceUID { get; set; }

        public DateTime? Sent { get; set; }

        public object Data { get; set; }

        public WSPayload()
        {
            id = Guid.NewGuid().ToString();
        }
    }

    // PAYLOAD TO ADD TO ABOVE CLASS DATA OBJECT...
    public class MistyPayload
    {
        public List<KeyValuePair<string, string>> Values { get; set; }
        public int Alerts { get; set; }

    }
}
