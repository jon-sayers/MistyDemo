using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public override MReading GetReading()
        {
            MReading output = new MReading();
            output.Id = "DEV-" + output.Id;
            output.TypeId = 1;

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
