using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace FunctionMisty
{
    public class HubFunction
    {
        private readonly MReadings _readings;

        public HubFunction(MReadings readings)
        {
            _readings = readings;
        }

        private static HttpClient client = new HttpClient();

        bool Exposure = true;

        [FunctionName("HubFunction")]
        public void Run([IoTHubTrigger("messages/events", Connection = "wezhub")]EventData message, [CosmosDB(
                databaseName: "wezmondoIOT",
                collectionName: "wezmondoIOT",
                ConnectionStringSetting = "wezdata")]out dynamic document, ILogger log)
        {
            string bodyString = Encoding.UTF8.GetString(message.Body.Array).Replace("Id","id");

            log.LogInformation($"Id: {JsonConvert.SerializeObject(message.SystemProperties["iothub-connection-device-id"])}");

            log.LogInformation($"C# IoT Hub trigger function processed a message: {bodyString}");

            document = bodyString;

            MReading reading = JsonConvert.DeserializeObject<MReading>(bodyString);

            _readings.AddReading(reading);

            log.LogInformation($"Readings: {_readings.Readings.Count()}");

            checkExposure(message.SystemProperties["iothub-connection-device-id"].ToString());
        }

        public async void checkExposure(string deviceId)
        {
            if (_readings.Readings.Count() % 10 == 0)
            {
                ServiceClient _serviceClient = ServiceClient.CreateFromConnectionString(System.Environment.GetEnvironmentVariable("wezhubhost"));

                var method = new CloudToDeviceMethod("Alert", TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(0));

                if (_readings.Readings.Count() % 20 == 0)
                {
                    method.SetPayloadJson("{\"exposure\":true}");
                }
                else
                {
                    method.SetPayloadJson("{\"exposure\":false}");
                }
                    
                var response = await _serviceClient.InvokeDeviceMethodAsync(deviceId, method);

                _serviceClient.Dispose();
            }


        }
    }
}