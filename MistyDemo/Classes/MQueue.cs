using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MistyDemo.Classes
{
    public class MQueue
    {
        // THREAD SAFE QUEUE SINGLETON USED THROUGHOUT APPLICATION...

        private readonly ConcurrentQueue<MReading> _readings = new ConcurrentQueue<MReading>();

        public MReading GetQueued()
        {
            _readings.TryDequeue(out MReading reading);
           
            return reading;        
        }

        public void Queue(MReading reading)
        {
            _readings.Enqueue(reading);          
        }

    }
}
