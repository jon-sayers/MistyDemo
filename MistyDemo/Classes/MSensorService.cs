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

        private ISensor _sensor;
        private MQueue _queue;
        private MDevice _device;

        public MSensorService(IWebHostEnvironment env, MQueue queue, MDevice device)
        {
            if (env.IsDevelopment())
            {
                _sensor = new MSensorDev();
            }
            else
            {
                _sensor = new MSensor();
            }
            _queue = queue;
            _device = device;
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
            while (true)
            {
                await Task.Delay(_device.Interval * 1000);              
                MReading reading = _sensor.GetReading();
                Console.WriteLine(JsonConvert.SerializeObject(reading));
                _queue.Queue(reading);
            }
        }


    }
}
