using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FunctionMisty
{
    public class MReadings
    {
        public MReadings()
        {
            Readings = new List<MReading>();
        }

        public List<MReading> Readings { get; set; }

        public void AddReading(MReading reading)
        {
            Readings.Add(reading);

            if (Readings.Count > 100)
            {
                Readings.RemoveAt(0);
            }
        }

    }

    public class MReading
    {
        public MReading()
        {
        }

        public string Id { get; set; }
        public int TypeId { get; set; }
        public DateTime Time { get; set; }
        public List<KeyValuePair<string, string>> Payload { get; set; }
        public string Hash { get; set; }
        public int Alerts { get; set; }
        
    }


}
