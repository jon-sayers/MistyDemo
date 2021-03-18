using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MistyDemo.Classes
{
    public class MReading
    {
        public MReading()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
    }
}
