using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
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
        private MQueue _queue { get; set; }
        private MDevice _device { get; set; }

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

            await _client.SetDesiredPropertyUpdateCallbackAsync(TwinFunction, null);

            await _client.SetMethodHandlerAsync("Alert", AlertFunction, null);
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

                    message.Properties.Add("Alert", (1 > 2) ? "true" : "false");

                    await _client.SendEventAsync(message);

                    Console.WriteLine("Sent: " + JsonConvert.SerializeObject(toSend));
                }
            }
        }

        private static Task<MethodResponse> AlertFunction(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);

            if (methodRequest.Data != null)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Alert: {data}");
                Console.ResetColor();

                // Acknowlege the direct method call with a 200 success message
                string result = $"{{\"result\":\"Executed direct method: {methodRequest.Name}\"}}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
            }
            else
            {
                // Acknowlege the direct method call with a 400 error message
                string result = "{\"result\":\"Invalid parameter\"}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
            }
        }

        private async Task TwinFunction(TwinCollection desiredProperties, object userContext)
        {
            string newIntervalString = desiredProperties["interval"];
            int newInterval = int.Parse(newIntervalString);

            _device.Interval = newInterval;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Sensor Interval Changed to {newIntervalString} seconds");
            Console.ResetColor();

            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties["DateTimeLastDesiredPropertyChangeReceived"] = DateTime.UtcNow;
            await _client.UpdateReportedPropertiesAsync(reportedProperties);

        }
    }
}
