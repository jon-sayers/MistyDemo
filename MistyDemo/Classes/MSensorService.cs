using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MistyDemo.Classes
{


    public class MSensorService : BackgroundService
    {

        private Thread _readThread { get; set; }
        private MQueue _queue { get; set; }
        private MDevice _device { get; set; }

        public MSensorService(IWebHostEnvironment env, MQueue queue, MDevice device)
        {

            _queue = queue;
            _device = device;

            _device.Interval = 5;

            // USE MOCK CLASS FOR TESTING...

            if (env.IsDevelopment())
            {
                _device.Sensor = new MSensorDev();
            }
            else
            {
                _device.Sensor = new MSensor();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                await this.Setup();
            }
        }

        private async Task Setup()
        {

            Console.WriteLine("Sensor Ready...");
            Console.WriteLine("Read Interval = " + _device.Interval);

            _readThread = new Thread(ReadLoop);
            _readThread.Start();
        }

        private async void ReadLoop()
        {
            // READ SENSOR AT DEFINED INTERVAL, AND QUEUE THE READING FOR SENDING TO THE HUB...

            while (true)
            {
                await Task.Delay(_device.Interval * 1000);              
                MReading reading = _device.Sensor.GetReading();
                Console.WriteLine("Queued:" + JsonConvert.SerializeObject(reading));
                Console.WriteLine();
                _queue.Queue(reading);
            }
        }


    }
}
