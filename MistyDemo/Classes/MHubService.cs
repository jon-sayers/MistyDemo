using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Configuration;
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
        private static string _conString;

        private Thread _sendThread { get; set; }
        private MQueue _queue { get; set; }
        private static MDevice _device { get; set; }

        private static IWebHostEnvironment _env;


        public MHubService(IWebHostEnvironment env, IConfiguration config, MQueue queue, MDevice device)
        {
            _queue = queue;
            _device = device;
            _env = env;
            _conString = config.GetConnectionString("Hub");
        }

        // STARTING POINT FOR BACKGROUND SERVICES...
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

            Twin twin = await _client.GetTwinAsync();

            string alertString = twin.Properties.Desired["alerts"].ToString();

            Console.WriteLine(alertString);

            _device.Alerts = JsonConvert.DeserializeObject<List<MAlert>>(alertString);

            await _client.SetDesiredPropertyUpdateCallbackAsync(TwinFunction, null);

            await _client.SetMethodHandlerAsync("Alert", AlertFunction, null);

            _sendThread = new Thread(SendLoop);

            _sendThread.Start();
        }

        private async void SendLoop()
        {
            // PULL READINGS OFF THE QUEUE AND SEND THEM TO THE HUB...

            while (true)
            {
                MReading toSend = _queue.GetQueued();

                if (toSend != null)
                {
                    
                    int alertCount = _device.CheckReading(toSend);

                    if (!_env.IsDevelopment())
                    {
                        _device.UpdatePins();
                    }
                        
                    toSend.Alerts = alertCount;

                    string toSendString = JsonConvert.SerializeObject(toSend);

                    using var message = new Message(Encoding.ASCII.GetBytes(toSendString))
                    {
                        ContentType = "application/json",
                        ContentEncoding = "utf-8",
                    };

                    message.Properties.Add("alerts", alertCount.ToString());

                    await _client.SendEventAsync(message);

                    if (alertCount > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                        Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine("Sent: " + JsonConvert.SerializeObject(toSend));

                    Console.ResetColor();

                }
            }
        }


        private static Task<MethodResponse> AlertFunction(MethodRequest methodRequest, object userContext)
        {
            // LISTENER FOR ALERT FUNCTION...

            var data = Encoding.UTF8.GetString(methodRequest.Data);

            if (methodRequest.Data != null)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Alert: {data}");
                Console.ResetColor();

                dynamic dataObject = JsonConvert.DeserializeObject(data);

                _device.Exposure = dataObject.exposure;


                if (!_env.IsDevelopment())
                {
                    _device.UpdatePins();
                }

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
            // LISTENER FOR TWIN UPDATES...

            string newIntervalString = desiredProperties["interval"];
            int newInterval = int.Parse(newIntervalString);
            _device.Interval = newInterval;

            string alertString = desiredProperties["alerts"].ToString();
            _device.Alerts = JsonConvert.DeserializeObject<List<MAlert>>(alertString);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Properties Updated from Twin");
            Console.ResetColor();

            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties["DateTimeLastDesiredPropertyChangeReceived"] = DateTime.UtcNow;
            await _client.UpdateReportedPropertiesAsync(reportedProperties);

        }
    }
}
