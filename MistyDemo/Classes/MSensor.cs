using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unosquare.RaspberryIO;

namespace MistyDemo.Classes
{
    public class MSensor: ISensor
    {
        public MSensor()
        {
            RecentReadings = new List<MReading>();
        }

        public List<MReading> RecentReadings { get; set;}

        public virtual MReading GetReading()
        {
            // Work to do here, search i2c bus for device, go through manual for sensor and find out the command to send, and how many bytes to expect back...

            //var myDevice = Pi.I2C.AddDevice(0x20);
            //myDevice.Write(0x44);
            //var response = myDevice.Read();

            MReading output = new MReading();
            return output;
        }

        public string GetReadingString()
        {
            return JsonConvert.SerializeObject(GetReading());
        }

    }

    public class MSensorDev : MSensor
    {
        // MOCK CLASS FOR TESTING...

        public override MReading GetReading()
        {
            MReading output = new MReading();
            output.Id = "DEV-" + output.Id;
            output.TypeId = 1;

            output.Payload.Add(new KeyValuePair<String, String>("p1", "300"));
            output.Payload.Add(new KeyValuePair<String, String>("p2", "250"));
            output.Payload.Add(new KeyValuePair<String, String>("p3", "400"));
            output.Payload.Add(new KeyValuePair<String, String>("p4", "400"));

            output.Time = DateTime.UtcNow;

            RecentReadings.Add(output);

            if (RecentReadings.Count > 10)
            {
                RecentReadings.RemoveAt(0);
            }

            output.CreateHash();

            return output;
        }

    }

    public interface ISensor
    {
        public List<MReading> RecentReadings { get; set; }

        public MReading GetReading();

        public string GetReadingString();

    }
}
