using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MistyDemo.Classes
{
    public class MSensor: ISensor
    {
        public MReading GetReading()
        {
            MReading output = new MReading();
            return output;
        }

        public string GetReadingString()
        {
            return JsonConvert.SerializeObject(GetReading());
        }

    }

    public class MSensorDev : ISensor
    {
        public MReading GetReading()
        {
            MReading output = new MReading();
            output.Id = "DEV-" + output.Id;
            return output;
        }

        public string GetReadingString()
        {
            return JsonConvert.SerializeObject(GetReading());
        }

    }

    public interface ISensor
    {
        public MReading GetReading();

        public string GetReadingString();

    }
}
