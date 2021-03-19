using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MistyDemo.Classes
{
    public class MDevice
    {
        public int Interval { get; set; }
        public ISensor Sensor { get; set; }
        public List<MAlert> Alerts { get; set; }

        public MDevice()
        {
            Alerts = new List<MAlert>();
        }
    }

    public class MAlert
    {
        public int TypeId { get; set; }
        public string Key { get; set; }
        public string Rule { get; set; }
        public string Alert { get; set; }
        public int Action { get; set; }
        public bool Eval { get; set; }

        public MAlert()
        {

        }
    }
}
