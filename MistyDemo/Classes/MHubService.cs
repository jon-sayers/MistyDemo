using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MistyDemo.Classes
{
    public class MHubService : BackgroundService
    {
        private static DeviceClient _client;
        private static string _conString = "HostName=wezmondo.azure-devices.net;DeviceId=Misty01;SharedAccessKey=8/gRFZW5LkhahVM41Dbh9UH7c0o1B0ttOtz0yIvrqHU=";

        private Thread _sendThread { get; set; }

        private MQueue _queue;
        private MDevice _device;

        public MHubService(MQueue queue, MDevice device)
        {
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
            Console.WriteLine("Starting Hub...");

            _client = DeviceClient.CreateFromConnectionString(_conString, TransportType.Mqtt);

            _sendThread = new Thread(SendLoop);
            _sendThread.Start();
        }

        private async void SendLoop()
        {
            while (true)
            {
                MReading toSend = _queue.GetQueued();

                if (toSend != null)
                {
                    string toSendString = JsonConvert.SerializeObject(toSend);

                    using var message = new Message(Encoding.ASCII.GetBytes(toSendString))
                    {
                        ContentType = "application/json",
                        ContentEncoding = "utf-8",
                    };

                    message.Properties.Add("rulename", (1 > 2) ? "true" : "false");

                    await _client.SendEventAsync(message);

                    Console.WriteLine("Sent: " + JsonConvert.SerializeObject(toSend));
                }
            }
        }

    }
}
